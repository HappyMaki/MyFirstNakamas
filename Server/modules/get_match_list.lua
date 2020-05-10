local nk = require("nakama")
local function get_match_list(context, payload)
  local decoded = nk.json_decode(payload)
  local limit = 1000
  local authoritative = nil 
  local label = decoded.label
  local min_size = 0
  local max_size = 1000
  local matches = nk.match_list(limit, authoritative, label, min_size, max_size)
  
  -- design choice. only one match is allowed per label. for now anyway.
  assert(#matches <= 1)

  if #matches == 0 then
    return "Error: " .. label .. "does not exist in match list"
  end
  
  for _, m in ipairs(matches)
  do
    return m.match_id
  end
end
nk.register_rpc(get_match_list, "get_match_list_rpc")
