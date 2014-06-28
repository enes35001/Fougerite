﻿namespace RustPP.Commands
{
    using Zumwalt;
    using RustPP;
    using System;
    using System.IO;

    public class RulesCommand : ChatCommand
    {
        public override void Execute(ref ConsoleSystem.Arg Arguments, ref string[] ChatArguments)
        {
            if (File.Exists(Helper.GetAbsoluteFilePath("rules.txt")))
            {
                foreach (string str in File.ReadAllLines(Helper.GetAbsoluteFilePath("rules.txt")))
                {
                    Util.sayUser(Arguments.argUser.networkPlayer, str);
                }
            }
            else
            {
                Util.sayUser(Arguments.argUser.networkPlayer, "No rules are currently set.");
            }
        }
    }
}