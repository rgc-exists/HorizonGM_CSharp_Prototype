
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UndertaleModLib.Decompiler;
using UndertaleModLib.Models;
using UndertaleModLib;
using Newtonsoft.Json;
using GMHooker;
using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static UndertaleModLib.Compiler.Compiler.AssemblyWriter;
using System.IO;
using static UndertaleModLib.Models.UndertaleRoom;
using System.Xml.Linq;

namespace GMMK
{
    public class GMMK_Patcher
    {
        public static Dictionary<string, string> files = new Dictionary<string, string>();

        public static UndertaleData data;

        public static string TEMP_HARDCODED_EXE_DIRECTORY = @"C:\Program Files (x86)\Steam\steamapps\common\Will You Snail\Will You Snail.exe";
        public static string TEMP_HARDCODED_OUT_DATA_DIRECTORY = @"C:\Program Files (x86)\Steam\steamapps\common\Will You Snail\cache.win";

        public static string gmmkDir;
        public static string modsPath;
        public static string baseDir;
        public static string patcherDir;
        public static string gmlCodeDir;

        private static string curLoadingCodeDir;

        string[] modDirs;
        public static bool failedHook = false;

        public static UndertaleData blankData;

        private UndertaleCode firstRoomInit;
        public void Load(UndertaleData moddingData, string[] exeArgs)
        {
            patcherDir = Environment.CurrentDirectory;
            gmmkDir = Path.GetDirectoryName(patcherDir);
            modsPath = Path.Combine(gmmkDir!, "mods");
            baseDir = Path.GetDirectoryName(gmmkDir);
            modDirs = Directory.GetDirectories(modsPath);
            gmlCodeDir = Path.Combine(patcherDir, "gmlCode");

            data = moddingData;
            blankData = UndertaleData.CreateNew();




            Console.WriteLine($"[GMMK]: Adding objects...");
            AddObjects();




            Console.WriteLine($"[GMMK]: Adding code...");
            LoadCode(gmlCodeDir);

            Console.WriteLine($"[GMMK]: Loading user mods...");
            bool successful = LoadMods();

            if (successful)
            {
                Console.WriteLine($"[GMMK]: Adding late code...");
                AddCodeAfterMods();

                data.FinalizeHooks();

                FileStream stream = File.OpenWrite(TEMP_HARDCODED_OUT_DATA_DIRECTORY);
                UndertaleIO.Write(stream, data);
                stream.Dispose();
            } else
            {
                Console.WriteLine("\n\n\n\n[GMMK]: Loading mods was not successful. Launching vanilla game...");
                Console.ReadLine();
            }
            

            StartGame(successful, exeArgs);

        }


        public bool LoadMods()
        {
            try
            {
                modDirs = Directory.GetDirectories(modsPath);


                Dictionary<string, List<string>> modJsonPaths = new Dictionary<string, List<string>>();
                List<string> allJsonPaths = new List<string>();
                List<string> allModNames = new List<string>();

                string scr_loadAssetsAsLocals_string = "";
                Dictionary<string, GameObjectData> gameObjects = new Dictionary<string, GameObjectData>();
                foreach (string modFolderPath in modDirs)
                {
                    Console.WriteLine("Loading user mod " + modFolderPath);
                    string[] modFiles = Directory.GetFileSystemEntries(modFolderPath);

                    List<string> jsonPaths = new List<string>();
                    List<string> spritePaths = new List<string>();
                    List<string> soundPaths = new List<string>();
                    List<string> directoryJsonPaths = new List<string>();
                    List<string> directoryJsonEarlyPaths = new List<string>();

                    foreach (string file in modFiles)
                    {
                        Console.WriteLine(Path.GetFileName(file));
                        if (file.EndsWith(".json"))
                        {
                            if (Path.GetFileName(file) == "directories.json")
                            {
                                directoryJsonPaths.Add(file);

                            } else if (Path.GetFileName(file) == "directories_early.json")
                            {
                                directoryJsonEarlyPaths.Add(file);

                            } else
                            {
                                jsonPaths.Add(file);
                                allJsonPaths.Add(file);

                            }
                        }
                    }

                    foreach (string file in directoryJsonEarlyPaths)
                    {
                        Console.WriteLine($"directories_early.json file found: {file}");
                        LoadCode(Directory.GetParent(file).FullName, "directories_early.json");
                    }

                    for (int stage = 1; stage <= 5; stage++)
                    {
                        foreach (string jsonF in jsonPaths)
                        {
                            Console.WriteLine("Loading json " + jsonF + " stage " + stage.ToString());
                            string jsonString = File.ReadAllText(jsonF);
                            var jsonData = JsonConvert.DeserializeObject<dynamic>(jsonString);
                            string jsonFLocation = Path.GetDirectoryName(jsonF);
                            if (jsonData != null)
                            {
                                if (stage == 1)
                                {
                                    if (jsonData.sprites != null)
                                    {
                                        foreach (var sprite in jsonData.sprites)
                                        {
                                            string spritePath = FindFileInOneOfTheseDirectories(
                                                new string[]{
                                            modFolderPath
                                                },
                                                sprite.path.ToString()
                                            );
                                            if (spritePath != null)
                                            {
                                                spritePaths.Add(spritePath);
                                                scr_loadAssetsAsLocals_string += sprite.name + " = global.GMMK_sprite_" + sprite.name + "\n";
                                            }
                                            else
                                            {
                                                Console.WriteLine("Could not find sprite path " + sprite.path.ToString() + " in json " + jsonF);
                                            }
                                        }
                                    }
                                    if (jsonData.sounds != null)
                                    {
                                        foreach (var sound in jsonData.sounds)
                                        {
                                            string soundPath = FindFileInOneOfTheseDirectories(
                                                new string[]{
                                            modFolderPath
                                                },
                                                sound.path.ToString()
                                            );

                                            scr_loadAssetsAsLocals_string += sound.name + " = global.GMMK_sound_" + sound.name + "\n";
                                            if (soundPath != null)
                                            {
                                                soundPaths.Add(soundPath);
                                            }
                                            else
                                            {
                                                Console.WriteLine("Could not find sprite path " + sound.path.ToString() + " in json " + jsonF);
                                            }
                                        }
                                    }
                                }
                                else if (stage == 2)
                                {
                                    if (jsonData.functions != null)
                                    {
                                        foreach (var function in jsonData.functions)
                                        {
                                            string functionPath = FindFileInOneOfTheseDirectories(
                                                new string[]{
                                                    modFolderPath
                                                },
                                                function.path.ToString()
                                            );
                                            string functionName = function.name.ToString();
                                            if (functionPath != null)
                                            {
                                                string code = File.ReadAllText(functionPath);
                                                MatchCollection matchList = Regex.Matches(code, @"(?<=argument)\d+");
                                                ushort argCount;
                                                if (matchList.Count > 0)
                                                    argCount = (ushort)(matchList.Cast<Match>().Select(match => ushort.Parse(match.Value)).ToList().Max() + 1);
                                                else
                                                    argCount = 0;
                                                data.CreateFunction(functionName, code, argCount);
                                            }
                                            else
                                            {
                                                Console.WriteLine("Could not find function path " + function.path.ToString() + " in json " + jsonF);
                                            }
                                        }
                                    }
                                }
                                else if (stage == 3 || stage == 4)
                                {
                                    if (jsonData.game_objects != null)
                                    {
                                        foreach (var objData in jsonData.game_objects)
                                        {
                                            if (stage == 3)
                                            {
                                                string parentName = objData.parent;
                                                UndertaleGameObject parentId = data.GameObjects.ByName(parentName);

                                                bool solid = objData.solid;

                                                string spriteName = objData.sprite;
                                                bool spriteIsCustom = false;

                                                UndertaleSprite sprite = null;
                                                if (spriteName != null)
                                                {
                                                    sprite = data.Sprites.ByName(spriteName);
                                                    if (sprite == null)
                                                    {
                                                        spriteIsCustom = true;
                                                    }
                                                }
                                                else
                                                {
                                                }

                                                string objName = objData.name;
                                                if (objName == null)
                                                {
                                                    Console.WriteLine("One of the objects located in " + jsonF + " does not have a name.");
                                                    continue;
                                                }

                                                bool? visible = objData.visible;
                                                if (visible == null)
                                                {
                                                    visible = true;
                                                }
                                                bool? persistent = objData.persistent;
                                                if (persistent == null)
                                                {
                                                    persistent = false;
                                                }

                                                UndertaleGameObject newGameObject = new UndertaleGameObject
                                                {
                                                    Name = data.Strings.MakeString(objName),
                                                    ParentId = parentId,
                                                    Solid = solid,
                                                    Sprite = sprite,
                                                    Visible = (bool)visible,
                                                    Persistent = (bool)persistent
                                                };
                                                data.GameObjects.Add(newGameObject);

                                                var addToStartRoom = objData.add_to_start_room;

                                                if (addToStartRoom != null)
                                                {
                                                    UndertaleRoom ogFirstRoom = data.GeneralInfo.RoomOrder[1].Resource;
                                                    // This takes into account the fact that the mod loader inserts its own room to be the "first room" in the room order.

                                                    data.GeneralInfo.LastObj++;
                                                    UndertaleRoom.GameObject newObjInstance = new UndertaleRoom.GameObject()
                                                    {
                                                        InstanceID = data.GeneralInfo.LastObj,
                                                        ObjectDefinition = newGameObject,
                                                        X = addToStartRoom.x != null ? addToStartRoom.x : 0,
                                                        Y = addToStartRoom.y != null ? addToStartRoom.y : 0,
                                                        ScaleX = addToStartRoom.x_scale != null ? addToStartRoom.x_scale : 1,
                                                        ScaleY = addToStartRoom.y_scale != null ? addToStartRoom.y_scale : 1,
                                                        Rotation = addToStartRoom.rotation != null ? addToStartRoom.rotation : 0,
                                                    };

                                                    ogFirstRoom.Layers[0].InstancesData.Instances.Add(newObjInstance);

                                                    ogFirstRoom.GameObjects.Add(newObjInstance);
                                                }

                                                GameObjectData newObjData = new GameObjectData
                                                {
                                                    undertaleGameObject = newGameObject,
                                                    spriteIsCustom = spriteIsCustom,
                                                    sprite = spriteName != null ? spriteName : "",
                                                    code = new List<CodeData>(),
                                                    addToStartRoom = addToStartRoom != null
                                                };

                                                gameObjects.Add(objName.ToString(), newObjData);

                                            }
                                            else if (stage == 4)
                                            {
                                                string objName = objData.name.ToString();
                                                UndertaleGameObject newGameObject = data.GameObjects.ByName(objName);


                                                List<CodeData> codeDatas = new List<CodeData>();
                                                Dictionary<string, string> codeEvents = objData.code.ToObject<Dictionary<string, string>>();

                                                foreach (string codeName in codeEvents.Keys.ToList())
                                                {
                                                    var codeFileName = codeEvents[codeName];
                                                    string codePath = FindFileInOneOfTheseDirectories(
                                                        new string[]{
                                                    modFolderPath
                                                        },
                                                        codeFileName
                                                    );


                                                    string codeFContents = File.ReadAllText(codePath);
                                                    EventType type = EventType.Create;
                                                    uint subtype = 0;
                                                    var validEventNames = Databases.EventNames.Keys.ToList();

                                                    UndertaleCode newCode = new UndertaleCode(); //Temporary placeholder since it's not nullable
                                                    if (validEventNames.Contains(codeName.Trim(), StringComparer.OrdinalIgnoreCase))
                                                    {
                                                        bool hasFound = false;
                                                        foreach (string eventName in validEventNames)
                                                        {
                                                            if (eventName.ToLower() == codeName.ToLower())
                                                            {
                                                                var typesData = Databases.EventNames[eventName];
                                                                type = typesData.Item1;
                                                                subtype = typesData.Item2;
                                                                hasFound = true;
                                                                break;
                                                            }
                                                        }

                                                        if (!hasFound)
                                                        {
                                                            throw new Exception("The code name \"" + codeName + "\" was invalid, even though it was found in the EventNames database. THIS SHOULD NOT HAPPEN! Please report this bug!");
                                                        }

                                                        newCode = newGameObject.EventHandlerFor(type, subtype, data.Strings, data.Code, data.CodeLocals);

                                                        newCode.ReplaceGML(codeFContents, data);
                                                    }
                                                    else if (codeName.ToLower().StartsWith("collision"))
                                                    {
                                                        List<string> splitUnderscores = codeName.Replace(" ", "_").Split("_").ToList();
                                                        splitUnderscores.RemoveAt(0);
                                                        string colliderName = string.Join("_", splitUnderscores);
                                                        UndertaleGameObject collidingObject = data.GameObjects.ByName(colliderName);
                                                        if (collidingObject != null)
                                                        {
                                                            newCode = newGameObject.EventHandlerFor(EventType.Collision, (uint)data.GameObjects.IndexOf(collidingObject), data.Strings, data.Code, data.CodeLocals);

                                                            newCode.ReplaceGML(codeFContents, data);
                                                        }
                                                        else
                                                        {
                                                            throw new Exception($"Tried to create a collision event between objects \"{objData.Name}\" and \"{colliderName}\",\nbut {colliderName} does not exist as an object!");
                                                        }
                                                    }
                                                    else
                                                    {
                                                        switch (codeName.ToLower())
                                                        {

                                                            case "wired":
                                                            case "powered":
                                                                MatchCollection matchList = Regex.Matches(codeFContents, @"(?<=argument)\d+");
                                                                ushort argCount;
                                                                if (matchList.Count > 0)
                                                                    argCount = (ushort)(matchList.Cast<Match>().Select(match => ushort.Parse(match.Value)).ToList().Max() + 1);
                                                                else
                                                                    argCount = 0;
                                                                data.CreateFunction($"obj_{objName}_Wired_0", codeFContents, argCount);
                                                                break;

                                                            default:
                                                                string codeType = codeName.Split("_")[0];
                                                                string codeSubtype = "";
                                                                if (codeName.Split("_").Length > 1)
                                                                {
                                                                    codeSubtype = codeName.Split("_")[1];
                                                                }

                                                                if (codePath != "")
                                                                {
                                                                    type = (EventType)Enum.Parse(typeof(EventType), codeType);

                                                                    if (codeSubtype == null) subtype = 0;
                                                                    else
                                                                    {
                                                                        try
                                                                        {
                                                                            subtype = (uint)Enum.Parse(FindType("UndertaleModLib.Models.EventSubtype" + codeType), codeSubtype);
                                                                        }
                                                                        catch
                                                                        {
                                                                            try
                                                                            {
                                                                                subtype = uint.Parse(codeSubtype);
                                                                            }
                                                                            catch
                                                                            {
                                                                                throw new Exception("The code name \"" + codeName + "\" was invalid.");
                                                                            }
                                                                        }
                                                                    }



                                                                }
                                                                else
                                                                {
                                                                    Console.WriteLine("Could not find code file " + codeFileName + " for object " + objName);
                                                                }

                                                                newCode = newGameObject.EventHandlerFor(type, subtype, data.Strings, data.Code, data.CodeLocals);

                                                                newCode.ReplaceGML(codeFContents, data);
                                                                break;



                                                        }
                                                    }



                                                    CodeData code = new CodeData
                                                    {
                                                        path = codePath,
                                                        undertaleCode = newCode
                                                    };
                                                    codeDatas.Add(code);



                                                }

                                                gameObjects[objName].code = codeDatas;
                                            }
                                        }
                                    }
                                }
                                else if (stage == 5)
                                {
                                    if (jsonData.hooks != null)
                                    {
                                        foreach (var hookData in jsonData.hooks)
                                        {
                                            string hookName = hookData.name;
                                            string hookLocalPath = hookData.path;
                                            string hookPath = FindFileInOneOfTheseDirectories(
                                                new string[] { modFolderPath },
                                                hookLocalPath
                                            );
                                            if (File.Exists(hookPath))
                                            {
                                                string gmlCode = File.ReadAllText(hookPath);
                                                UndertaleCode code = data.Code.ByName(hookName);
                                                if (code == null)
                                                {
                                                    Console.WriteLine($"WARNING: {hookName} is not a valid name for a code or function. Source: {hookPath}");
                                                    continue;
                                                }
                                                if (code.ParentEntry == null)
                                                {
                                                    Console.WriteLine($"Adding code hook for {hookPath}");
                                                    data.HookCode(hookName, gmlCode);
                                                }
                                                else
                                                {
                                                    Console.WriteLine($"Adding function hook for {hookPath}");
                                                    data.HookFunction(hookName, gmlCode);
                                                }
                                            }
                                            else
                                            {
                                                Console.WriteLine($"WARNING: Path does not exist: {hookPath}");
                                            }
                                        }
                                    }

                                }
                            }
                            else
                            {
                                Console.WriteLine("Could not load json: " + jsonF);
                            }
                        }


                    }

                    allModNames.Add(Path.GetFileName(modFolderPath));
                    modJsonPaths.Add(Path.GetFileName(modFolderPath), jsonPaths);

                    foreach (string file in directoryJsonPaths)
                    {
                        Console.WriteLine($"directories.json file found: {file}");
                        LoadCode(Directory.GetParent(file).FullName);
                    }

                }

                string spriteInitStr = "#orig#()\n";
                foreach (GameObjectData objData in gameObjects.Values)
                {
                    if (objData.spriteIsCustom)
                    {
                        if (objData.sprite != "")
                        {
                            spriteInitStr += @$"
object_set_sprite({objData.undertaleGameObject.Name.Content}, global.GMMK_sprite_{objData.sprite})
";
                        }
                    }
                }
                data.HookCode(firstRoomInit.Name.Content, spriteInitStr.Replace("\\", "\\\\"));

                if (scr_loadAssetsAsLocals_string.Trim() == "")
                {
                    scr_loadAssetsAsLocals_string = "emptyFunctionsBreakUMT = 69420";
                }
                data.CreateFunction("scr_GMMK_loadAssetsAsLocals", scr_loadAssetsAsLocals_string, 0);

                foreach (UndertaleGameObject gameObject in data.GameObjects)
                {
                    // TODO: Make parent check recursive in case of objects with parents that also have parents.
                    string createCodeName = "gml_Object_" + gameObject.Name.Content + "_Create_0";

                    if (data.Code.ByName(createCodeName) != null)
                    {
                        data.HookCode(createCodeName, "gml_Script_scr_GMMK_loadAssetsAsLocals()\n#orig#()");
                    }
                    else
                    {
                        // TODO: Access the create functions directly rather than based on the name ending in "_Create_0", in case of custom names by other mods.
                        if (gameObject.ParentId != null && data.Code.ByName("gml_Object_" + gameObject.ParentId.Name.Content + "_Create_0") != null)
                        {
                            gameObject.EventHandlerFor(EventType.Create, data)
                            .ReplaceGML("gml_Script_scr_GMMK_loadAssetsAsLocals()\nevent_inherited()", data);
                        }
                        else
                        {
                            gameObject.EventHandlerFor(EventType.Create, data)
                            .ReplaceGML("gml_Script_scr_GMMK_loadAssetsAsLocals()", data);
                        }
                    }
                }

                data.CreateFunction("GMMK_initialize_info", @$"
global.GMMK_all_json_paths = {StringListToGMLString(allJsonPaths).Replace("\\", "\\\\")}
global.GMMK_mod_json_paths = {StringListDictionaryToGMLString(modJsonPaths).Replace("\\", "\\\\")}
global.GMMK_all_mod_names = {StringListToGMLString(allModNames).Replace("\\", "\\\\")}
global.GMMK_all_mod_paths = {StringListToGMLString(modDirs)}".Replace("\\", "\\\\"), 0);

                if (!failedHook)
                {
                    return true;
                }
            } catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return false;
            }
            return false;
        }

        private static string StringListDictionaryToGMLString(Dictionary<string, List<string>> dict)
        {
            string returnStr = $"gml_Script_GMMK_modhelper_array_to_struct([";
            for(int k = 0; k < dict.Count; k++)
            {
                string key = dict.Keys.ToArray()[k];
                List<string> arrayStr = dict[key];
                returnStr += $"\"{key}\", {StringListToGMLString(arrayStr)}";
                if (k < dict.Count - 1) returnStr += ", ";
            }
            returnStr += "])";

            return returnStr;
        }

        private static string StringListToGMLString(List<string> strs)
        {
            string returnStr = "[";
            for (int p = 0; p < strs.Count; p++)
            {
                string str = strs[p];
                returnStr += $"\"{str}\"";
                if (p < strs.Count - 1) returnStr += ", ";
            }
            returnStr += "]";

            return returnStr;
        }
        private static string StringListToGMLString(string[] strs)
        {
            return StringListToGMLString(strs.ToList());
        }

        private static Type? FindType(string qualifiedTypeName)
        {
            Type? t = Type.GetType(qualifiedTypeName);

            if (t != null) return t;

            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                t = asm.GetType(qualifiedTypeName);
                if (t != null)
                    return t;
            }
            return null;
        }


        public string FindFileInOneOfTheseDirectories(string[] directories, string path)
        {
            foreach (string dirPath in directories)
            {
                string newPath = Path.Combine(dirPath, path);
                if (File.Exists(newPath))
                {
                    return newPath;
                }
            }
            return null;
        }
        private static void LoadCode(string codePath, string jsonFileName = "directories.json")
        {

            Console.WriteLine($"[GMMK]: Loading code from files: {codePath}");
            files = LoadCodeFromFiles(codePath);

            curLoadingCodeDir = codePath;
            var directoryJsonPath = Path.Combine(codePath, jsonFileName);
            var directories = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(directoryJsonPath));
            if (directories == null) return;


            var handlers = SetupHandlers();

            foreach (var directory in directories)
            {
                var path = Path.Combine(codePath, directory.Key);
                if (!Directory.Exists(path))
                {
                    Console.WriteLine($"Cant find {path} skipping...");
                }
                else
                {
                    Console.WriteLine($"Loading code: {path}...");

                    if (!handlers.ContainsKey(directory.Value))
                    {
                        Console.WriteLine($"Path {path} has handler {directory.Value} which isn't in handlers");
                    }
                    else
                    {
                        foreach (var file in Directory.GetFiles(path))
                        {
                            Console.WriteLine($"Loading file: {file}...");
                            var code = File.ReadAllText(file);
                            code = ReplaceMacros(code);
                            
                            handlers[directory.Value].Invoke(code, file);
                        }
                    }
                }
            }


        }
        //Thanks to NameTheMPGuy for writing this code-loading code, originally used for Shellworks!
        private static Dictionary<string, Action<string, string>> SetupHandlers()
        {
            var handlers = new Dictionary<string, Action<string, string>>();

            handlers.Add("functions", (code, file) =>
            {
                var functionName = Path.GetFileNameWithoutExtension(file);

                MatchCollection matchList = Regex.Matches(code, @"(?<=argument)\d+");
                ushort argCount;
                if (matchList.Count > 0)
                    argCount = (ushort)(matchList.Cast<Match>().Select(match => ushort.Parse(match.Value)).ToList().Max() + 1);
                else
                    argCount = 0;
                Console.WriteLine("Creating new function " + functionName + " with argument count " + argCount.ToString());
                data.CreateFunction(functionName, code, argCount);
            });

            handlers.Add("codehooks", (code, file) =>
            {
                Console.WriteLine("Hooking to object code " + Path.GetFileNameWithoutExtension(file));
                data.HookCode(Path.GetFileNameWithoutExtension(file), code);
            });

            handlers.Add("functionhooks", (code, file) =>
            {
                Console.WriteLine("Hooking to function " + Path.GetFileNameWithoutExtension(file));
                data.HookFunction(Path.GetFileNameWithoutExtension(file), code);
            });

            handlers.Add("objectcode", (code, file) =>
            {
                if (file.EndsWith(".gml")) return;
                Console.WriteLine("Adding new object code " + Path.GetFileNameWithoutExtension(file));

                var objectFile = JsonConvert.DeserializeObject<ObjectFile>(code);
                if (objectFile == null) return;

                var type = (EventType)Enum.Parse(typeof(EventType), objectFile.Type);
                uint subtype;

                if (!objectFile.HasSubtype) subtype = uint.Parse(objectFile.Subtype);
                else subtype = (uint)Enum.Parse(FindType("UndertaleModLib.Models.EventSubtype" + objectFile.Type), objectFile.Subtype);


                string code_str = File.ReadAllText(Path.Combine(Path.GetDirectoryName(file), objectFile.File));
                code_str = ReplaceMacros(code_str);

                data.GameObjects.ByName(objectFile.Object)
                    .EventHandlerFor(type, subtype, data.Strings, data.Code, data.CodeLocals)
                    .ReplaceGML(code_str, data);
            });

            handlers.Add("inlinehooks", (code, file) =>
            {
                if (file.EndsWith(".gml")) return;

                var hookFile = JsonConvert.DeserializeObject<HookFile>(code);
                if (hookFile == null) return;

                Console.WriteLine("Adding inline hook(s) to " + file);


                foreach (HookFile_Hook hookData in hookFile.Hooks)
                {
                    UndertaleCode undertaleCode = data.Code.ByName(hookFile.Script);

                    string assembly_str = undertaleCode.Disassemble(data.Variables, data.CodeLocals.For(undertaleCode));
                    List<string> lines = assembly_str.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).ToList();

                    bool foundAtAll = false;
                    for (var l = 0; l < lines.Count; l++)
                    {
                        int cur_l = l;
                        int before = l;
                        var found = true;
                        for (int i = 0; i < hookData.Sig.Length && cur_l < lines.Count; i++)
                        {
                            if (lines[cur_l] == hookData.Sig[i]) { cur_l++; continue; }
                            found = false;
                            break;
                        }

                        if (!found) continue;

                        foundAtAll = true;
                        string fileContents = File.ReadAllText(Path.Combine(curLoadingCodeDir, hookData.File));
                        string scriptName = Path.GetFileNameWithoutExtension(hookData.File);
                        string functionArgs = "";
                        for (int a = 0; a < hookData.InArgs.Length; a++)
                        {
                            var argument = hookData.InArgs[a];
                            if (!argument.StartsWith("local"))
                            {
                                if (a < hookData.InArgs.Length - 1)
                                {
                                    functionArgs += argument.Replace("arg.", "") + ", ";
                                }
                                else
                                {
                                    functionArgs += argument.Replace("arg.", "");
                                }
                            }
                        }
                        Console.WriteLine("Adding inlineHook function for " + Path.GetFileNameWithoutExtension(hookData.File));
                        string hookStr = "function " + scriptName + "(" + functionArgs + "){\n" + fileContents + "\n}";
                        if (data.Code.ByName(Path.GetFileNameWithoutExtension(hookData.File)) == null)
                        {
                            data.CreateLegacyScript(Path.GetFileNameWithoutExtension(hookData.File), hookStr, (ushort)hookData.InArgs.Length);
                        }
                        if (hookData.Type == "replace")
                        {
                            lines.RemoveRange(before, hookData.Sig.Length);
                        }
                        int insertionIndex = hookData.Type == "append" ? cur_l : before;
                        int curInsertIndex = insertionIndex;
                        foreach (var argument in hookData.InArgs)
                        {
                            lines.Insert(curInsertIndex, argument.StartsWith("local") ? $"pushloc.v {argument}" : $"push.v {argument}");
                            curInsertIndex++;
                        }
                        int argCount = hookData.InArgs.Length;
                        lines.Insert(curInsertIndex, $"call.i gml_Script_{scriptName}(argc={argCount})");
                        curInsertIndex++;
                        lines.Insert(curInsertIndex, "popz.v");
                        curInsertIndex++;
                        string assemblyStr_out = "";
                        foreach (string li in lines)
                        {
                            assemblyStr_out += li + "\n";
                        }
                        assemblyStr_out = ReplaceAssetsWithIndexes_ASM(assemblyStr_out);
                        undertaleCode.Replace(Assembler.Assemble(assemblyStr_out, data));
                        if (hookData.Type == "prepend")
                        {
                            l = curInsertIndex;
                        }
                    }

                    if (!foundAtAll)
                    {

                        string assemblyLookedFor = "";
                        foreach (string l in hookData.Sig)
                        {
                            assemblyLookedFor += l + "\n";
                        }
                        Console.WriteLine("\n\nWARNING: could not find place to assembly hook for " + Path.GetFileNameWithoutExtension(file) + "\n" + assemblyLookedFor + "\n\n\n");
                        failedHook = true;
                    }
                }
            });
            handlers.Add("inlineassemblyhooks", (code, file) =>
            {
                if (file.EndsWith(".gml") || file.EndsWith(".asm")) return;

                var hookFile = JsonConvert.DeserializeObject<HookFile_Asm>(code);
                if (hookFile == null) return;

                Console.WriteLine("Adding inline assembly hook(s) to " + file);


                foreach (HookFile_Hook_Asm hookData in hookFile.Hooks)
                {
                    UndertaleCode undertaleCode = data.Code.ByName(hookFile.Script);

                    string assembly_str = undertaleCode.Disassemble(data.Variables, data.CodeLocals.For(undertaleCode)).Replace("\r\n", "\n").Replace("\r", "\n");


                    string find = hookData.ToFind;

                    string replace = hookData.ToReplace;
                    if (hookData.IsExternalFile)
                    {
                        replace = File.ReadAllText(Path.Combine(curLoadingCodeDir, hookData.File));
                    }

                    if (assembly_str.Contains(find))
                    {
                        string assemblyStr_out = assembly_str.Replace(find, replace);

                        assemblyStr_out = ReplaceAssetsWithIndexes_ASM(assemblyStr_out);
                        //Console.WriteLine(assemblyStr_out);
                        //Console.ReadLine();
                        undertaleCode.Replace(Assembler.Assemble(assemblyStr_out, data));
                    }
                    else
                    {
                        Console.WriteLine("\n\nWARNING: could not find place to assembly inline hook for " + Path.GetFileNameWithoutExtension(file) + "\n\n\n");
                        failedHook = true;
                    }
                }
            });
            handlers.Add("assemblies", (code, file) =>
            {
                UndertaleCode undertaleCode = data.Code.ByName(Path.GetFileNameWithoutExtension(file));
                string assemblyStr_out = ReplaceAssetsWithIndexes_ASM(code);
                if (undertaleCode == null)
                {
                    undertaleCode = data.CreateCode(data.Strings.MakeString(Path.GetFileNameWithoutExtension(file)), out var locals);

                    List<string> assemblyStr_lines = assemblyStr_out.Split("\n").ToList();
                    while (assemblyStr_lines.Count > 0 && assemblyStr_lines[0].Replace(" ", "").StartsWith(";gml_Script_"))
                    {
                        string line = assemblyStr_lines[0].Replace(" ", "");
                        string scriptName = line.TrimStart(';').Split(",")[0];
                        if (line.Contains(","))
                        {
                            string argCount_Str = line.Split(",")[1];
                            if (ushort.TryParse(argCount_Str, out ushort argCount))
                            {
                                UndertaleScript newScript = undertaleCode.CreateFunctionDefinition(data, true, scriptName.Replace("gml_Script_", ""), argCount, 0);
                                //GMHooker.CreateExtensions.CreateInlineFunction(undertaleCode, data, locals, scriptName.Replace("gml_Script_", ""), code, argCount);
                            }
                        }
                        assemblyStr_lines.RemoveAt(0);
                    }
                    assemblyStr_out = string.Join('\n', assemblyStr_lines.ToArray());
                }
                Console.WriteLine(assemblyStr_out);
                undertaleCode.Replace(Assembler.Assemble(assemblyStr_out, data));
            });
            handlers.Add("inlinereplacements", (code, file) =>
            {
                if (file.EndsWith(".gml")) return;

                var hookFile = JsonConvert.DeserializeObject<ReplaceFile>(code);
                if (hookFile == null) return;

                Console.WriteLine("Adding inline replacement(s) for " + Path.GetFileNameWithoutExtension(file));


                foreach (ReplaceFile_Replacement replaceData in hookFile.Replacements)
                {
                    UndertaleCode undertaleCode = data.Code.ByName(hookFile.Script);

                    string assembly_str = undertaleCode.Disassemble(data.Variables, data.CodeLocals.For(undertaleCode)).Replace("\r\n", "\n").Replace("\r", "\n").Replace("\t", "").Replace(" ", "");
                    GlobalDecompileContext globalDecompileContext = new GlobalDecompileContext(data, false);
                    string decompiledStr = Decompiler.Decompile(undertaleCode, globalDecompileContext);


                    string find = replaceData.ToFind;

                    string replace = replaceData.ToReplace;

                    if (replaceData.FindIsExternalFile)
                    {
                        find = File.ReadAllText(Path.Combine(gmlCodeDir, replaceData.FindFile)).Trim();
                    }
                    if (replaceData.ReplaceIsExternalFile)
                    {
                        replace = File.ReadAllText(Path.Combine(gmlCodeDir, replaceData.ReplaceFile)).Trim();
                    }
                    //find = find.Replace("\r\n", "\n").Replace("\r", "\n");
                    //replace = replace.Replace("\r\n", "\n").Replace("\r", "\n");

                    try
                    {
                        string decompiledStr_out = decompiledStr.Replace(find, replace);

                        undertaleCode.ReplaceGmlSafe(decompiledStr_out, data);

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message + "\n" + e.StackTrace);
                        Console.WriteLine("\n\nWARNING: could not find place to put inline replacement for " + Path.GetFileNameWithoutExtension(file) + "\n\n\n");
                        failedHook = true;
                    }
                }
            });

            return handlers;
        }

        private static string ReplaceAssetsWithIndexes_ASM(string assemblyStr)
        {
            string assemblyStr_out = assemblyStr;

            Regex regex = new Regex("UNINITIALIZED_PATTERN");

            MatchCollection objMatches = Regex.Matches(assemblyStr_out, @"#OBJECT_INDEX#\((.*)\)");
            foreach (Match match in objMatches)
            {
                assemblyStr_out = assemblyStr_out.Replace(@$"#OBJECT_INDEX#({match.Groups[1].Value})", data.GameObjects.IndexOf(data.GameObjects.ByName(match.Groups[1].Value)).ToString());
            }

            MatchCollection spriteMatches = Regex.Matches(assemblyStr_out, @"#SPRITE_INDEX#\((.*)\)");
            foreach (Match match in spriteMatches)
            {
                assemblyStr_out = assemblyStr_out.Replace(@$"#SPRITE_INDEX#({match.Groups[1].Value})", data.Sprites.IndexOf(data.Sprites.ByName(match.Groups[1].Value)).ToString());

            }

            MatchCollection soundMatches = Regex.Matches(assemblyStr_out, @"#SOUND_INDEX#\((.*)\)");
            foreach (Match match in soundMatches)
            {
                assemblyStr_out = assemblyStr_out.Replace(@$"#SOUND_INDEX#({match.Groups[1].Value})", data.Sounds.IndexOf(data.Sounds.ByName(match.Groups[1].Value)).ToString());

            }

            MatchCollection stringMatces = Regex.Matches(assemblyStr_out, @"""(.*)""@(.*)$");
            foreach (Match match in stringMatces)
            {
                assemblyStr_out = assemblyStr_out.Replace(@$"""{match.Groups[1].Value}""@{match.Groups[2].Value}", @$"""{match.Groups[1].Value}""@{data.Strings.IndexOf(data.Strings.MakeString(match.Groups[1].Value))}");

            }


            return assemblyStr_out;
        }


        private static string ReplaceMacros(string code)
        {
            var macroJsonPath = Path.Combine(gmlCodeDir, "macros.json");
            var macros = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(macroJsonPath));

            // We do a double pass to try to replace nested macros
            code = macros.Aggregate(code, (current, macro) => current.Replace(macro.Key, macro.Value));
            return macros.Aggregate(code, (current, macro) => current.Replace(macro.Key, macro.Value));
        }

        public static string[] GetFilesRecursively(string path)
        {
            List<string> pathsSoFar = new List<string>();

            foreach (var file in Directory.GetFiles(path))
            {
                pathsSoFar.Add(file);
            }

            foreach (var dir in Directory.GetDirectories(path))
            {
                pathsSoFar = GetFilesRecursively_Internal(dir, pathsSoFar);
            }

            return pathsSoFar.ToArray();
        }


        public static List<string> GetFilesRecursively_Internal(string path, List<string> pathsSoFar)
        {
            foreach (var file in Directory.GetFiles(path))
            {
                pathsSoFar.Add(file);
            }

            foreach (var dir in Directory.GetDirectories(path))
            {
                pathsSoFar = GetFilesRecursively_Internal(dir, pathsSoFar);
            }

            return pathsSoFar;
        }


        public void AddCodeAfterMods()
        {
            data.HookCode(firstRoomInit.Name.Content, "gml_Script_GMMK_initialize_info()\n" + File.ReadAllText(Path.Combine(gmlCodeDir, "hacky", "GMMK_firstRoom_init.gml")));
        }

        public void AddObjects()
        {
            /*
            UndertaleString name = new UndertaleString("obj_GMMK_manager");
            data.Strings.Add(name);
            UndertaleGameObject obj_GMMK_manager = new UndertaleGameObject()
            {
                Name = name,
                Visible = true,
                Solid = false
            };
            data.GameObjects.Add(obj_GMMK_manager);
            */

            UndertaleRoom ogFirstRoom = data.GeneralInfo.RoomOrder[0].Resource;
            UndertaleRoom firstRoom = new UndertaleRoom() { Name = data.Strings.MakeString("gmmk_startupRoom"), Caption = data.Strings.MakeString("") };

            firstRoom.Flags = firstRoom.Flags | RoomEntryFlags.IsGMS2;
            UndertaleResourceById<UndertaleRoom, UndertaleChunkROOM> roomResource = new UndertaleResourceById<UndertaleRoom, UndertaleChunkROOM>(firstRoom);
            data.Rooms.Add(firstRoom);
            data.GeneralInfo.RoomOrder.Insert(0, roomResource);

            firstRoomInit = data.CreateCode(data.Strings.MakeString("GMMK_firstRoom_init"), out var locals);
            firstRoomInit.ReplaceGML($"room_goto({ogFirstRoom.Name.Content})", data);
            firstRoom.CreationCodeId = firstRoomInit;

            // TODO: Make it so "On Game Startup" functions in objects in the OG first room still get called.

            //AddObjectToRoom(firstRoom.Name.Content, obj_GMMK_manager, firstRoom.Layers[0].LayerName.Content);
        }

        public UndertaleGameObject NewObject(string objectName, UndertaleSprite sprite = null, bool visible = true, bool solid = false, bool persistent = false, UndertaleGameObject parentObject = null)
        {
            UndertaleString name = new UndertaleString(objectName);
            UndertaleGameObject newObject = new UndertaleGameObject()
            {
                Sprite = sprite,
                Persistent = persistent,
                Visible = visible,
                Solid = solid,
                Name = name,
                ParentId = parentObject
            };

            data.Strings.Add(name);
            data.GameObjects.Add(newObject);

            return newObject;
        }

        public UndertaleRoom.GameObject AddObjectToRoom(string roomName, UndertaleGameObject objectToAdd, string layerName)
        {
            UndertaleRoom room = GetRoomFromData(roomName);

            UndertaleRoom.GameObject object_inst = new UndertaleRoom.GameObject()
            {
                InstanceID = data.GeneralInfo.LastObj,
                ObjectDefinition = objectToAdd,
                X = -120,
                Y = -120
            };
            data.GeneralInfo.LastObj++;

            room.Layers.First(layer => layer.LayerName.Content == layerName).InstancesData.Instances.Add(object_inst);


            room.GameObjects.Add(object_inst);

            return object_inst;
        }


        public UndertaleGameObject GetObjectFromData(string name)
        {
            return data.GameObjects.ByName(name);
        }
        public UndertaleSprite GetSpriteFromData(string name)
        {
            return data.Sprites.ByName(name);
        }
        public UndertaleRoom GetRoomFromData(string name)
        {
            return data.Rooms.ByName(name);
        }
        public UndertaleCode GetObjectCodeFromData(string name)
        {
            return data.Code.ByName(name);
        }
        public UndertaleFunction GetFunctionFromData(string name)
        {
            return data.Functions.ByName(name);
        }
        public UndertaleScript GetScriptFromData(string name)
        {
            return data.Scripts.ByName(name);
        }
        public UndertaleSound GetSoundFromData(string name)
        {
            return data.Sounds.ByName(name);
        }
        public UndertaleVariable GetVariableFromData(string name)
        {
            return data.Variables.ByName(name);
        }



        public void HookFunctionFromFile(string path, string function)
        {
            string value = "";
            if (files.TryGetValue(path, out value))
            {
                Console.WriteLine($"[GMMK]: loading {path}");
                data.HookFunction(function, value);
            }
            else
            {
                Console.WriteLine($"[GMMK]: Couldn't hook function {path}, it wasn't in the files dictionary.");
            }
        }
        public void CreateFunctionFromFile(string path, string function, ushort argumentCount = 0)
        {
            string value = "";
            if (files.TryGetValue(path, out value))
            {
                Console.WriteLine($"[GMMK]: loading {path}");
                data.CreateFunction(function, value, argumentCount);
            }
            else
            {
                Console.WriteLine($"[GMMK]: Couldn't create function {path}, it wasn't in the files dictionary.");
            }
        }

        public void HookCodeFromFile(string path, string function)
        {
            string value = "";
            if (files.TryGetValue(path, out value))
            {
                Console.WriteLine($"[GMMK]: loading {path}");
                data.HookCode(function, value);
            }
            else
            {
                Console.WriteLine($"[GMMK]: Couldn't hook object script {path}, it wasn't in the files dictionary.");
            }
        }


        public void CreateObjectCodeFromFile(string path, string objName, EventType eventType)
        {
            string value = "";
            UndertaleGameObject obj = data.GameObjects.ByName(objName);

            if (files.TryGetValue(path, out value))
            {
                obj.EventHandlerFor(eventType, data.Strings, data.Code, data.CodeLocals)
                .ReplaceGmlSafe(value, data);
            }
            else
            {
                Console.WriteLine($"[GMMK]: Couldn't change/create object script {path}, it wasn't in the files dictionary.");
            }
        }

        public void CreateObjectCodeFromFile(string path, string objName, EventType eventType, EventSubtypeDraw EventSubtype)
        {
            string value = "";
            UndertaleGameObject obj = data.GameObjects.ByName(objName);

            if (files.TryGetValue(path, out value))
            {
                obj.EventHandlerFor(eventType, EventSubtype, data.Strings, data.Code, data.CodeLocals)
                .ReplaceGmlSafe(value, data);
            }
            else
            {
                Console.WriteLine($"[GMMK]: Couldn't change/create object script {path}, it wasn't in the files dictionary.");
            }
        }
        public void CreateObjectCodeFromFile(string path, string objName, EventType eventType, uint EventSubtype)
        {
            string value = "";
            UndertaleGameObject obj = data.GameObjects.ByName(objName);

            if (files.TryGetValue(path, out value))
            {
                obj.EventHandlerFor(eventType, EventSubtype, data.Strings, data.Code, data.CodeLocals)
                .ReplaceGmlSafe(value, data);
            }
            else
            {
                Console.WriteLine($"[GMMK]: Couldn't change/create object script {path}, it wasn't in the files dictionary.");
            }
        }
        public void CreateObjectCodeFromFile(string path, string objName, EventType eventType, EventSubtypeKey EventSubtype)
        {
            string value = "";
            UndertaleGameObject obj = data.GameObjects.ByName(objName);

            if (files.TryGetValue(path, out value))
            {
                obj.EventHandlerFor(eventType, EventSubtype, data.Strings, data.Code, data.CodeLocals)
                .ReplaceGmlSafe(value, data);
            }
            else
            {
                Console.WriteLine($"[GMMK]: Couldn't change/create object script {path}, it wasn't in the files dictionary.");
            }
        }

        public void CreateObjectCodeFromFile(string path, string objName, EventType eventType, EventSubtypeMouse EventSubtype)
        {
            string value = "";
            UndertaleGameObject obj = data.GameObjects.ByName(objName);

            if (files.TryGetValue(path, out value))
            {
                obj.EventHandlerFor(eventType, EventSubtype, data.Strings, data.Code, data.CodeLocals)
                .ReplaceGmlSafe(value, data);
            }
            else
            {
                Console.WriteLine($"[GMMK]: Couldn't change/create object script {path}, it wasn't in the files dictionary.");
            }
        }


        public void CreateObjectCodeFromFile(string path, string objName, EventType eventType, EventSubtypeOther EventSubtype)
        {
            string value = "";
            UndertaleGameObject obj = data.GameObjects.ByName(objName);

            if (files.TryGetValue(path, out value))
            {
                obj.EventHandlerFor(eventType, EventSubtype, data.Strings, data.Code, data.CodeLocals)
                .ReplaceGmlSafe(value, data);
            }
            else
            {
                Console.WriteLine($"[GMMK]: Couldn't change/create object script {path}, it wasn't in the files dictionary.");
            }
        }

        public void CreateObjectCodeFromFile(string path, string objName, EventType eventType, EventSubtypeStep EventSubtype)
        {
            string value = "";
            UndertaleGameObject obj = data.GameObjects.ByName(objName);

            if (files.TryGetValue(path, out value))
            {
                obj.EventHandlerFor(eventType, EventSubtype, data.Strings, data.Code, data.CodeLocals)
                .ReplaceGmlSafe(value, data);
            }
            else
            {
                Console.WriteLine($"[GMMK]: Couldn't change/create object script {path}, it wasn't in the files dictionary.");
            }
        }

        public static Dictionary<string, string> LoadCodeFromFiles(string path)
        {
            Dictionary<string, string> files = new Dictionary<string, string>();
            string[] codeF = Directory.GetFiles(path, "*.gml");
            Console.WriteLine($"[GMMK]: Loading code from {path}");
            foreach (string f in codeF)
            {
                if (!files.ContainsKey(Path.GetFileName(f)))
                {
                    files.Add(Path.GetFileName(f), File.ReadAllText(f));
                }
            }
            return files;
        }

        public static void StartGame(bool loadmods, string[] args)
        {
            ProcessStartInfo processStartInfo = new()
            {
                FileName = TEMP_HARDCODED_EXE_DIRECTORY,
                WorkingDirectory = Directory.GetParent(TEMP_HARDCODED_EXE_DIRECTORY).FullName
            };

            processStartInfo.ArgumentList.Add("-game");

            if (loadmods)
                processStartInfo.ArgumentList.Add(TEMP_HARDCODED_OUT_DATA_DIRECTORY);
            else
                processStartInfo.ArgumentList.Add("data.win");

            for (var i = 1; i < args.Length; i++)
            {
                processStartInfo.ArgumentList.Add(args[i]);
            }

            Process.Start(processStartInfo);
        }

    }
}
