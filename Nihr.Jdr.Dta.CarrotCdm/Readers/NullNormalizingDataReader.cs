using System.Data;

namespace Nihr.Jdr.Dta.CarrotCdm.Readers;

public sealed class NullNormalizingDataReader(IDataReader inner) : IDataReader
{
    public object GetValue(int i)
    {
        var value = inner.GetValue(i);

        if (value is string s && string.IsNullOrWhiteSpace(s))
        {
            return DBNull.Value;
        }

        return value;
    }

    public int GetValues(object[] values) => inner.GetValues(values);

    public bool Read() => inner.Read();
    public int FieldCount => inner.FieldCount;

    public object this[int i] => GetValue(i);
    public object this[string name] => GetValue(GetOrdinal(name));

    public bool IsDBNull(int i) => GetValue(i) == DBNull.Value;

    public string GetName(int i) => inner.GetName(i);
    public int GetOrdinal(string name) => inner.GetOrdinal(name);
    public Type GetFieldType(int i) => inner.GetFieldType(i);

    public void Close() => inner.Close();
    public DataTable? GetSchemaTable() => inner.GetSchemaTable();
    public bool NextResult() => inner.NextResult();
    public int Depth => inner.Depth;
    public bool IsClosed => inner.IsClosed;
    public int RecordsAffected => inner.RecordsAffected;
    public void Dispose() => inner.Dispose();

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
        => inner.GetBytes(i, fieldOffset, buffer, bufferoffset, length);

    public long GetChars(int i, long fieldoffset, char[]? buffer, int bufferoffset, int length)
        => inner.GetChars(i, fieldoffset, buffer, bufferoffset, length);

    public IDataReader GetData(int i) => inner.GetData(i);
    public string GetDataTypeName(int i) => inner.GetDataTypeName(i);
}