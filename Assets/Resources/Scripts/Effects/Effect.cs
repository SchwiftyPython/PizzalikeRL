using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Effect
{
    protected string _name;

    protected Guid _id;

    protected int _duration;

    public string Name
    {
        get
        {
            if (_name != null && !_name.Equals(string.Empty))
            {
                return _name;
            }

            var fullName = GetType().FullName;

            if (fullName != null)
            {
                _name = fullName.Substring(fullName.LastIndexOf('.') + 1);
            }

            return _name;
        }
    }
}
