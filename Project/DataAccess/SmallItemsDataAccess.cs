using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class SmallItemsDataAccess
{
    private static string _filePath =
        System.IO.Path.GetFullPath(System.IO.Path.Combine(Environment.CurrentDirectory,
            @"DataSources/smallItems.json"));

    private static GenericJsonAccess<SmallItemsModel> _smallItemsModel =
        new GenericJsonAccess<SmallItemsModel>(_filePath);


    public static List<SmallItemsModel> LoadAll()
    {
        return _smallItemsModel.LoadAll();
    }

    public static void WriteAll(List<SmallItemsModel> smallItems)
    {
        _smallItemsModel.WriteAll(smallItems);
    }
}