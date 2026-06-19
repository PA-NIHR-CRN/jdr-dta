using System.Data;
using System.Globalization;

namespace Nihr.Jdr.Dta.Infrastructure.DataReaders;

public sealed class OmopDateFixingReader : IDataReader
{
    private readonly IDataReader _inner;
    private readonly Dictionary<int, int> _datePairs;
    private readonly HashSet<int> _dateColumnIndices;

    public OmopDateFixingReader(IDataReader inner)
    {
        this._inner = inner;
        (_datePairs, _dateColumnIndices) = AnalyzeColumns(inner);
    }

    public object GetValue(int i)
    {
        var raw = NormalizeNull(_inner.GetValue(i));

        // 1. Only attempt parsing if this column is known to be a date or datetime field
        if (_dateColumnIndices.Contains(i) && raw is string s)
        {
            if (TryParseOmopDate(s, out var parsed))
            {
                return parsed;
            }
        }

        // 2. Fallback logic: If a *_date field is null, try to derive it from the *_datetime field
        if (_datePairs.TryGetValue(i, out var datetimeIdx) && raw == DBNull.Value)
        {
            var dtRaw = NormalizeNull(_inner.GetValue(datetimeIdx));

            if (dtRaw is string dtString && TryParseOmopDate(dtString, out var parsed))
            {
                return parsed.Date;
            }

            if (dtRaw is DateTime dt)
            {
                return dt.Date;
            }
        }

        return raw;
    }

    public int GetValues(object[] values) => _inner.GetValues(values);

    public bool Read() => _inner.Read();
    public int FieldCount => _inner.FieldCount;

    public object this[int i] => GetValue(i);
    public object this[string name] => GetValue(GetOrdinal(name));

    public bool IsDBNull(int i) => GetValue(i) == DBNull.Value;

    public string GetName(int i) => _inner.GetName(i);
    public int GetOrdinal(string name) => _inner.GetOrdinal(name);
    public Type GetFieldType(int i) => _inner.GetFieldType(i);

    public void Close() => _inner.Close();
    public DataTable? GetSchemaTable() => _inner.GetSchemaTable();
    public bool NextResult() => _inner.NextResult();
    public int Depth => _inner.Depth;
    public bool IsClosed => _inner.IsClosed;
    public int RecordsAffected => _inner.RecordsAffected;
    public void Dispose() => _inner.Dispose();

    public bool GetBoolean(int i) => (bool)GetValue(i);
    public byte GetByte(int i) => (byte)GetValue(i);
    public char GetChar(int i) => (char)GetValue(i);
    public DateTime GetDateTime(int i) => (DateTime)GetValue(i);
    public decimal GetDecimal(int i) => (decimal)GetValue(i);
    public double GetDouble(int i) => (double)GetValue(i);
    public float GetFloat(int i) => (float)GetValue(i);
    public Guid GetGuid(int i) => (Guid)GetValue(i);
    public short GetInt16(int i) => (short)GetValue(i);
    public int GetInt32(int i) => (int)GetValue(i);
    public long GetInt64(int i) => (long)GetValue(i);
    public string GetString(int i) => (string)GetValue(i);

    public long GetBytes(int i, long fieldOffset, byte[]? buffer, int bufferoffset, int length)
        => _inner.GetBytes(i, fieldOffset, buffer, bufferoffset, length);

    public long GetChars(int i, long fieldoffset, char[]? buffer, int bufferoffset, int length)
        => _inner.GetChars(i, fieldoffset, buffer, bufferoffset, length);

    public IDataReader GetData(int i) => _inner.GetData(i);
    public string GetDataTypeName(int i) => _inner.GetDataTypeName(i);

    private static object NormalizeNull(object value)
    {
        if (value is string s && string.IsNullOrWhiteSpace(s))
        {
            return DBNull.Value;
        }

        return value;
    }

    private static Dictionary<int, int> FindDatePairs(IDataReader reader)
    {
        var nameToIndex = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        for (var i = 0; i < reader.FieldCount; i++)
        {
            nameToIndex[reader.GetName(i)] = i;
        }

        var pairs = new Dictionary<int, int>();

        foreach (var (name, dateIdx) in nameToIndex)
        {
            if (!name.EndsWith("_date", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var datetimeName = name.Replace("_date", "_datetime", StringComparison.OrdinalIgnoreCase);

            if (nameToIndex.TryGetValue(datetimeName, out var datetimeIdx))
            {
                pairs[dateIdx] = datetimeIdx;
            }
        }

        return pairs;
    }

    private static (Dictionary<int, int> Pairs, HashSet<int> DateIndices) AnalyzeColumns(IDataReader reader)
    {
        var nameToIndex = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        var dateIndices = new HashSet<int>();
        var pairs = new Dictionary<int, int>();

        for (var i = 0; i < reader.FieldCount; i++)
        {
            var name = reader.GetName(i);
            nameToIndex[name] = i;

            if (name.EndsWith("_date", StringComparison.OrdinalIgnoreCase) || 
                name.EndsWith("_datetime", StringComparison.OrdinalIgnoreCase))
            {
                dateIndices.Add(i);
            }
        }

        foreach (var (name, dateIdx) in nameToIndex)
        {
            if (!name.EndsWith("_date", StringComparison.OrdinalIgnoreCase)) continue;

            var datetimeName = name.Replace("_date", "_datetime", StringComparison.OrdinalIgnoreCase);
            if (nameToIndex.TryGetValue(datetimeName, out var datetimeIdx))
            {
                pairs[dateIdx] = datetimeIdx;
            }
        }

        return (pairs, dateIndices);
    }
    
    private static bool TryParseOmopDate(string value, out DateTime result)
    {
        // Try standard parsing first
        if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
        {
            return true;
        }

        // Try the specific yyyyMMdd format used in some vocabulary files
        return DateTime.TryParseExact(
            value,
            "yyyyMMdd",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out result);
    }
}