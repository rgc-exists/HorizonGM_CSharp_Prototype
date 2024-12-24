var modPaths = global.GMMK_all_mod_paths
var modNames = global.GMMK_all_mod_names
var modJsonInfos = global.GMMK_mod_json_paths


for(var mp = 0; mp < array_length(modPaths); mp++){
    var modPath = modPaths[mp]
    var modName = modNames[mp]
    var jsonPaths = variable_struct_get(modJsonInfos, modName)
    for(var j = 0; j < array_length(jsonPaths); j++){
        var jsonP = jsonPaths[j]
        var jsonFile = file_text_open_read(jsonP)
        var jsonStr = ""
        while(!file_text_eof(jsonFile)){
            jsonStr += file_text_read_string(jsonFile)
            file_text_readln(jsonFile)
        }

        var json_data = json_parse(jsonStr)
        if(variable_struct_exists(json_data, "sprites")){
            var sprites = json_data.sprites 
            for(var s = 0; s < array_length(sprites); s++){
                var sprite_data = sprites[s]
                if(!variable_struct_exists(sprite_data, "number_of_images")) sprite_data.number_of_images = 1
                var new_sprite = sprite_add(modPath + "/" + sprite_data.path, sprite_data.number_of_images, false, false, sprite_data.xorigin, sprite_data.yorigin)
                variable_global_set("GMMK_sprite_" + sprite_data.name, new_sprite)
            }

            //global sprite var format = global.GMMK_sprite_[SPRITE NAME]
        }
    }
}

#orig#()