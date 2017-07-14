using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[Serializable]
public class DrawingTemplateModel
{
    public string _id;
    public string name;
    public string image;
    public string thumb;
    public string _v;
}

[Serializable]
public class DrawingTemplateListModel
{
    public int success;
    public string dir;
    public List<DrawingTemplateModel> data;
    public DrawingTemplateListModel()
    {
        data = new List<DrawingTemplateModel>(0);
    }
    public DrawingTemplateModel Get(int i)
    {
        if (data == null) return null;
        return data[i];
    }
    public int Count()
    {
        if (data == null) return 0;
        return data.Count;
    }
}



