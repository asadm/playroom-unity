using SimpleJSON;
using UnityEngine;

namespace Playroom
{
    /// <summary>
    /// This file contains functions, mostly used for serialization / deserialization 
    /// </summary>
    public partial class PlayroomKit
    {
        private static string SerializeInitOptions(InitOptions options)
        {
            if (options == null) return null;

            JSONNode node = new JSONObject();

            node["streamMode"] = options.streamMode;
            node["allowGamepads"] = options.allowGamepads;
            node["baseUrl"] = options.baseUrl;

            if (options.avatars != null)
            {
                JSONArray avatarsArray = new JSONArray();
                foreach (string avatar in options.avatars)
                {
                    avatarsArray.Add(avatar);
                }

                node["avatars"] = avatarsArray;
            }

            node["roomCode"] = options.roomCode;
            node["skipLobby"] = options.skipLobby;
            node["reconnectGracePeriod"] = options.reconnectGracePeriod;

            // Serialize matchmaking field
            if (options.matchmaking is bool booleanMatchmaking)
            {
                node["matchmaking"] = booleanMatchmaking;
            }
            else if (options.matchmaking is MatchMakingOptions matchmakingOptions)
            {
                JSONNode matchmakingNode = new JSONObject();
                matchmakingNode["waitBeforeCreatingNewRoom"] = matchmakingOptions.waitBeforeCreatingNewRoom;
                node["matchmaking"] = matchmakingNode;
            }

            if (options.maxPlayersPerRoom.HasValue)
            {
                node["maxPlayersPerRoom"] = options.maxPlayersPerRoom.Value;
            }

            if (options.gameId != null)
            {
                node["gameId"] = options.gameId;
            }

            node["discord"] = options.discord;

            if (options.defaultStates != null)
            {
                JSONObject defaultStatesObject = new JSONObject();
                foreach (var kvp in options.defaultStates)
                {
                    defaultStatesObject[kvp.Key] = ConvertValueToJSON(kvp.Value);
                }

                node["defaultStates"] = defaultStatesObject;
            }

            if (options.defaultPlayerStates != null)
            {
                JSONObject defaultPlayerStatesObject = new JSONObject();
                foreach (var kvp in options.defaultPlayerStates)
                {
                    defaultPlayerStatesObject[kvp.Key] = ConvertValueToJSON(kvp.Value);
                }

                node["defaultPlayerStates"] = defaultPlayerStatesObject;
            }

            return node.ToString();
        }

        private static JSONNode ConvertValueToJSON(object value)
        {
            if (value is string stringValue)
            {
                return stringValue;
            }
            else if (value is int intValue)
            {
                return intValue;
            }
            else if (value is float floatValue)
            {
                return floatValue;
            }
            else if (value is bool boolValue)
            {
                return boolValue;
            }
            else
            {
                // Handle other types if needed
                return JSON.Parse("{}");
            }
        }

        private static JSONArray CreateJsonArray(string[] array)
        {
            JSONArray jsonArray = new JSONArray();

            foreach (string item in array)
            {
                jsonArray.Add(item);
            }

            return jsonArray;
        }

        private static string ConvertJoystickOptionsToJson(JoystickOptions options)
        {
            JSONNode joystickOptionsJson = new JSONObject();
            joystickOptionsJson["type"] = options.type;

            // Serialize the buttons array
            JSONArray buttonsArray = new JSONArray();
            foreach (ButtonOptions button in options.buttons)
            {
                JSONObject buttonJson = new JSONObject();
                buttonJson["id"] = button.id;
                buttonJson["label"] = button.label;
                buttonJson["icon"] = button.icon;
                buttonsArray.Add(buttonJson);
            }

            joystickOptionsJson["buttons"] = buttonsArray;

            // Serialize the zones property (assuming it's not null)
            if (options.zones != null)
            {
                JSONObject zonesJson = new JSONObject();
                zonesJson["up"] = ConvertButtonOptionsToJson(options.zones.up);
                zonesJson["down"] = ConvertButtonOptionsToJson(options.zones.down);
                zonesJson["left"] = ConvertButtonOptionsToJson(options.zones.left);
                zonesJson["right"] = ConvertButtonOptionsToJson(options.zones.right);
                joystickOptionsJson["zones"] = zonesJson;
            }

            return joystickOptionsJson.ToString();
        }

        // Function to convert ButtonOptions to JSON
        private static JSONNode ConvertButtonOptionsToJson(ButtonOptions button)
        {
            JSONObject buttonJson = new JSONObject();
            buttonJson["id"] = button.id;
            buttonJson["label"] = button.label;
            buttonJson["icon"] = button.icon;
            return buttonJson;
        }

        private static Player.Profile ParseProfile(string json)
        {
            var jsonNode = JSON.Parse(json);
            var profileData = new Player.Profile();
            profileData.playerProfileColor = new Player.Profile.PlayerProfileColor
            {
                r = jsonNode["color"]["r"].AsInt,
                g = jsonNode["color"]["g"].AsInt,
                b = jsonNode["color"]["b"].AsInt,
                hexString = jsonNode["color"]["hexString"].Value,
                hex = jsonNode["color"]["hex"].AsInt
            };

            ColorUtility.TryParseHtmlString(profileData.playerProfileColor.hexString, out UnityEngine.Color color1);
            profileData.color = color1;
            profileData.name = jsonNode["name"].Value;
            profileData.photo = jsonNode["photo"].Value;

            return profileData;
        }
    }
}