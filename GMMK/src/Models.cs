using UndertaleModLib.Models;

namespace GMMK;
class GameObjectData
{
    public UndertaleGameObject undertaleGameObject { get; set; }
    public List<CodeData> code { get; set; }
    public bool spriteIsCustom { get; set; }
    public string sprite { get; set; }
}

class CodeData
{
    public string path { get; set; }
    public UndertaleCode undertaleCode { get; set; }
}