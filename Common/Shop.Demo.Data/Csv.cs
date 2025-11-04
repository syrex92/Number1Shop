namespace Shop.Demo.Data;

internal class Csv
{
    private readonly List<string> _columns = [];

    private readonly List<string[]> _rows = [];

    public string[] Columns => _columns.ToArray();
    public string[] Row(int row) => _rows[row];

    public string Cell(string column, int row)
    {
        var idx = _columns.IndexOf(column);
        if (idx == -1)
            return string.Empty;

        var rowValues = _rows[row];

        if (idx >= rowValues.Length)
            return string.Empty;

        return rowValues[idx];
    }

    public void Set(string column, int row, string value)
    {
        var idx = _columns.IndexOf(column);
        if (idx == -1)
            throw new IndexOutOfRangeException();

        var rowValues = _rows[row];

        if (idx >= rowValues.Length)
            throw new IndexOutOfRangeException();

        rowValues[idx] = value;
    }

    public string[] ColumnValues(string column)
    {
        var idx = _columns.IndexOf(column);
        if (idx == -1)
            return [];

        var values = _rows.Select(x => x[idx]).ToArray();
        return values;
    }

    public void SetHeader(string[] names)
    {
        _columns.AddRange(names);
    }

    public void AddValues(string[] values)
    {
        _rows.AddRange(values);
    }

    public int ColumnCount => _columns.Count;
    public int RowCount => _rows.Count;
}