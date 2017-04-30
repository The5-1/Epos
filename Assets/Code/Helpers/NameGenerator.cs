using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Text; //stringbuilder
using System; //char.toUpper

using Random = UnityEngine.Random;

public static class NameGenerator{


    public static string RandomName(bool female = false)
    {
        List<string> phonemes = new List<string>(new string[] {"ron", "gus","he","ma","pu","ko","tam","ad","min","er", "el", "re", "is","tra","tor","po","io","iff","oh","ho","nan","tok","gah","sli", "ei","ter","mi","la","he","tro","eck","st","ri","to","mas","hum","an","fa","da","ja","to","rem", "gr", "re", "lem", "tu", "rok", "nil", "tek","ton","bum","boo","ber","zer","ker","rg","am","na","fe","fo","la","li","fu","tu","ra","ma","rah","reh","que","ze","coa","tl","te","noc","ti","cl","an","ti","ris"});
        List<string> title = new List<string>(new string[] { "Sir", "Ser", "Lord", "Loht", "Mr.", "Prince", "Guru", "Poor", "Old", "Young", "Handsome", "Amun"});
        List<string> title_female = new List<string>(new string[] { "Ms.", "Lady", "Lord", "Mr."});
        List<string> nickname_part1 = new List<string>(new string[] { "O\'", "Hammer", "Bee", "Wall", "Blood", "Doom", "Cannon", "Moon", "Star", "God", "Dood", "Dude", "Arrow", "Sword", "Bone", "Skull", "Corpse","Wind"});
        List<string> nickname_part2 = new List<string>(new string[] { "", "hugger", "thirst", "lie", "seer", "moon", "eater", "lord", "god", "sword", "blade", "fire", "wind", "devourer"});
        List<string> spacer = new List<string>(new string[] { "-", " "});
        /*
        const List<string> phonemes = new List<string>();
        phonemes.Add("ha");
        phonemes.Add("re");
        */

        StringBuilder firstname = new StringBuilder();
        StringBuilder secondname = new StringBuilder();
        StringBuilder lastname = new StringBuilder();

        StringBuilder nickname = new StringBuilder();

        StringBuilder final = new StringBuilder();

        bool only_first = Random.Range(0, 100) > 90 ? true : false;
        bool has_title = Random.Range(0, 100) > 90 ? true : false;
        bool has_second = Random.Range(0, 100) > 80 ? true : false;
        bool has_nickname = Random.Range(0, 100) > 80 ? true : false;

        int maxLength = Random.Range(2, 4) + Random.Range(1, 3);

        int length_first = Random.Range(1, maxLength);
        int length_last = Random.Range(1, maxLength);
        if (only_first) //if no last make first longer
        {
            length_first += length_last -1;
        }

        //append Title
        if (has_title)
        {
            final.Append(title[Random.Range(0, title.Count)]);
            final.Append(" ");
        }

        //build first
        for (int i = 0; i < length_first; i++)
        {
            int random = Random.Range(0, phonemes.Count);
            firstname.Append(phonemes[random]);
        }
        firstname[0] = Char.ToUpper(firstname[0]);
        final.Append(firstname);

        if(!only_first)
        { 
            //build second
            if (has_second)
            {
                final.Append(spacer[Random.Range(0, spacer.Count)]);

                int length_second = Random.Range(1, maxLength);
                for (int i = 0; i < length_second; i++)
                {
                    int random = Random.Range(0, phonemes.Count);
                    secondname.Append(phonemes[random]);
                }
                secondname[0] = Char.ToUpper(secondname[0]);
                final.Append(secondname);
            }

            //build nickname
            if (has_nickname)
            {
                final.Append(" ");
                final.Append(nickname_part1[Random.Range(0, nickname_part1.Count)]);
                final.Append(nickname_part2[Random.Range(0, nickname_part2.Count)]);
            }

            //build last
            final.Append(" ");
            for (int i = 0; i < length_last; i++)
            {
                int random = Random.Range(0, phonemes.Count);
                lastname.Append(phonemes[random]);
            }
            lastname[0] = Char.ToUpper(lastname[0]);
            final.Append(lastname);
        }
    
    
        return final.ToString();
    }

}
