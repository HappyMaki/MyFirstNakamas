local nk = require("nakama")

local M = {}

local API_BASE_URL = "https://pokeapi.co/api/v2"

function M.lookup_pokemon(name)
  local url = ("%s/pokemon/%s"):format(API_BASE_URL, name)
  local method = "GET"
  local headers = {
    ["Content-Type"] = "application/json",
    ["Accept"] = "application/json"
  }
  local success, code, _, body = pcall(nk.http_request, url, method, headers, nil)
  if (not success) then
    nk.logger_error(("Failed request %q"):format(code))
    error(code)
  elseif (code >= 400) then
    nk.logger_error(("Failed request %q %q"):format(code, body))
    error(body)
  else
    return nk.json_decode(body)
  end
end

return M