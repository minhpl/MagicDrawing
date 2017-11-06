using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class UserListModel
{
    public List<UserModel> userModels;
    public UserListModel(List<UserModel> userModels)
    {
        this.userModels = userModels;
    }

    public UserListModel()
    {
        this.userModels = new List<UserModel>();
        this.userModels.Add(new UserModel("Người chơi 1"));
    }

    public void Add(UserModel um)
    {
        if (this.userModels == null)
        {
            this.userModels = new List<UserModel>();
        }
        this.userModels.Add(um);
    }
    public void Update(UserModel um)
    {
        if (this.userModels == null)
        {
            this.userModels = new List<UserModel>();
            this.userModels.Add(um);
        }
        else
        {
            for (int i = 0; i < this.userModels.Count; i++)
            {
                if (this.userModels[i].id.Equals(um.id))
                {
                    this.userModels[i] = um;
                }
            }
        }
    }

    public bool Delete(int i)
    {
        if (this.userModels == null)
        {
            return false;
        }
        if (i < 0 || i >= userModels.Count) return false;
        userModels.RemoveAt(i);
        return true;
    }

    public UserModel Get(int i)
    {
        if (this.userModels != null && i >= 0 && i < this.userModels.Count)
        {
            return this.userModels[i];
        }
        else
        {
            return new UserModel("Người chơi");
        }
    }
    public void Set(UserModel um, int i)
    {
        this.userModels[i] = um;
    }

    public int Count()
    {
        if (userModels == null) return 0;
        return userModels.Count;
    }

}

[Serializable]
public class UserModel
{
    public string id;
    public string name;
    public string avata;
    public int survivalHighScore = 0;
    public UserModel(string name)
    {
        id = "" + DateTime.Now.Ticks;
        this.name = name;
    }

    public UserModel Clone()
    {
        UserModel um = new UserModel(name);
        um.id = this.id;
        um.avata = this.avata;
        return um;
    }
}
