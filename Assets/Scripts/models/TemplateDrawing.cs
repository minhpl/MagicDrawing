using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[Serializable]
public class TemplateDrawing
{
    public string _id;
    public string name;
    public string image;
    public string thumb;
    public string _v;
}

[Serializable]
public class TemplateDrawingList
{
    public int success;
    public string dir;
    public List<TemplateDrawing> templates;
    public TemplateDrawingList()
    {
        templates = new List<TemplateDrawing>(0);
    }
    public TemplateDrawing Get(int i)
    {
        if (templates == null) return null;
        return templates[i];
    }
    public int Count()
    {
        if (templates == null) return 0;
        return templates.Count;
    }
}



