local nk = require("nakama")

local function create_match(context, payload)
  local decoded = nk.json_decode(payload)
  
  local modulename = decoded.modulename
  -- local setupstate = { initialstate = decoded.label }
  local setupstate = { initialstate = decoded }
  local matchid = nk.match_create(modulename, setupstate)

  -- Send notification of some kind
  return matchid
end
nk.register_rpc(create_match, "create_match_rpc")
