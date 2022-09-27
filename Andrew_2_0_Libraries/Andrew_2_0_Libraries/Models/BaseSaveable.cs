using System;

namespace Andrew_2_0_Libraries.Models
{
    public abstract class BaseSaveable
    {
        public abstract string GetTextOutput();
        public abstract bool ParseData(string data);
    }
}
