local nk = require("nakama")
local match_info = require("match_info")

local function create_match(context, payload)
  local decoded = nk.json_decode(payload)

  local limit = 1000
  local authoritative = nil 
  local min_size = 0
  local max_size = 1000
  local matches = nk.match_list(limit, authoritative, label, min_size, max_size)

  if #matches ~= 0 then
    return "nil"
  end

  local modulename = decoded.modulename
  local setupstate = { initialstate = decoded }
  local matchid = nk.match_create(modulename, setupstate)

  return matchid
end

nk.register_rpc(create_match, "create_match_rpc")
