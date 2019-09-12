﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Effect
{
    protected string name;

    protected Guid id;

    protected int duration;

    protected Entity entity;

    public string Name
    {
        get
        {
            if (name != null && !name.Equals(string.Empty))
            {
                return name;
            }

            var fullName = GetType().FullName;

            if (fullName != null)
            {
                name = fullName.Substring(fullName.LastIndexOf('.') + 1);
            }

            return name;
        }
    }
}
