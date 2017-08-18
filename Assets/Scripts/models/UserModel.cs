using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class UserListModel
{
    public UserModel[] userModels;
    public UserListModel(UserModel[] userModels)
    {
        this.userModels = userModels;
    }

    public UserListModel()
    {
        this.userModels = new UserModel[1];
        this.userModels[0] = new UserModel("Người chơi 1");
    }

    public void Add(UserModel um)
    {
        if (this.userModels == null)
        {
            this.userModels = new UserModel[1];
            this.userModels[0] = um;
        }
        else
        {
            UserModel[] arr = new UserModel[this.userModels.Length + 1];
            this.userModels.CopyTo(arr, 0);
            arr[arr.Length - 1] = um;
            this.userModels = arr;
        }
    }
    public void Update(UserModel um)
    {
        if (this.userModels == null)
        {
            this.userModels = new UserModel[1];
            this.userModels[0] = um;
        }
        else
        {
            for (int i = 0; i < this.userModels.Length; i++)
            {
                if (this.userModels[i].id.Equals(um.id))
                {
                    this.userModels[i] = um;
                }
            }
        }
    }

    public UserModel Get(int i)
    {
        if (this.userModels != null && i >= 0 && i < this.userModels.Length)
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

}

[Serializable]
public class UserModel
{
    public string id;
    public string name;
    public string avata;
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
