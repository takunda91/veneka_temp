namespace Veneka.Indigo.Common.Language
{
    public interface IResponseTranslator
    {
        string TranslateResponseCode(SystemResponseCode responseCode, SystemArea systemArea, int language, long auditUserId, string auditWorkstation);
    }
}