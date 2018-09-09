namespace NonStandartQuery
{
    //// Этот класс позволяет обрабатывать определенные события в классе параметров:
    //// Событие SettingChanging возникает перед изменением значения параметра.
    //// Событие PropertyChanged возникает после изменения значения параметра.
    //// Событие SettingsLoaded возникает после загрузки значений параметров.
    //// Событие SettingsSaving возникает перед сохранением значений параметров.

    internal sealed class Settings
    {
        public static object Default { get; internal set; }
    }
}
