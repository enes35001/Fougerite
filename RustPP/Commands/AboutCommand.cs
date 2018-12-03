namespace RustPP.Commands
{
    using Fougerite;
    using RustPP;
    using System;

    public class AboutCommand : ChatCommand
    {
        public override void Execute(ref ConsoleSystem.Arg Arguments, ref string[] ChatArguments)
        {
            var pl = Fougerite.Server.Cache[Arguments.argUser.userID];
            pl.MessageFrom(Core.Name, "Fougerite Şuanda Rust++ Sistemiyle Çalışıyor Versiyon" + Core.Version);
            pl.MessageFrom(Core.Name, "Türkçe Yama Enes Şevluk Trafından Yapılmış Yaratıcı ise DreTaX");
        }
    }
}
