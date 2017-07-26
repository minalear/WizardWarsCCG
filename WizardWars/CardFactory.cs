using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace WizardWars
{
    public static class CardFactory
    {
        public static List<CardInfo> LoadCards(string jsonText)
        {
            return JsonConvert.DeserializeObject<List<CardInfo>>(jsonText);
        }
    }
}
