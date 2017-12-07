using System;
using System.Collections.Generic;

[Serializable]
public class CategoryRequest
{
    public string categoryId;
    public CategoryRequest(string id)
    {
        this.categoryId = id;
    }
}

[Serializable]
public class Category
{
    public string _id;
    public string name;
    public string image;
    public int _v;
}

[Serializable]
public class CategoryList
{
    public int success;
    public string dir;
    public List<Category> data;

    public CategoryList()
    {
        data = new List<Category>();
    }

    public int Count()
    {
		if (data == null)
			return 0;
        return data.Count;
    }
}

public class Frame
{
    public string id;
    public string thumb;
    public string image;
    public int _v;
}

public class FrameList
{
    public int success;
    public string dir;
    public List<Frame> data;
}