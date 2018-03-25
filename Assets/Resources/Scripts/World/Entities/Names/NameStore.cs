using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NameStore
{

    private List<string> _nameFiles = new List<string>
    {
        "human_male_first_names_normal.csv",
        "human_female_first_names_normal.csv",
        "human_male_first_names_odd.csv",
        "human_last_names_odd.csv",
        "human_last_names_normal.csv",
        "human_female_first_names_odd.csv",
        "dwarf_male_first_names_normal.csv",
        "dwarf_last_names_odd.csv",
        "dwarf_last_names_normal.csv",
        "dwarf_female_first_names_normal.csv"
    };

    private const string _namesPath = "Assets\\Resources\\Scripts\\World\\Entities\\Names";

    public void Initialize()
    {
        
    }
}
