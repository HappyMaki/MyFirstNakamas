local nk = require("nakama")

local M = {}

local OP_LOOP = 1
local OP_TERMINATE = 2
local OP_POSITION_DATA = 3

function M.match_init(context, setupstate)
  local gamestate = {
    presences = {},
    positions = {}
  }
  local tickrate = 30 -- per sec
  local label = ""
  return gamestate, tickrate, label
end

function M.match_join_attempt(context, dispatcher, tick, state, presence, metadata)
  local acceptuser = true
  print("someone joined attempt!")
  return state, acceptuser
end

function M.match_join(context, dispatcher, tick, state, presences)
  -- print(context);
  for _, presence in ipairs(presences) do
    state.presences[presence.session_id] = presence
    -- state.positions[presence.session_id] = 
    -- print("Presence: ")
    -- print(presence.session_id)
  end
  -- print("someone joined!")
  return state
end

function M.match_leave(context, dispatcher, tick, state, presences)
  for _, presence in ipairs(presences) do
    state.presences[presence.session_id] = nil
  end
  -- print("someone left!")
  return state
end

function M.match_loop(context, dispatcher, tick, state, messages)
  -- for _, presence in pairs(state.presences) do
    -- print(("Presence %s named %s"):format(presence.user_id, presence.username))
  -- end
  for _, message in ipairs(messages) do
    -- print(("Received %s from %s"):format(message.data, message.sender.username))
    -- print( table.tostring( message.sender ) )
    local decoded = nk.json_decode(message.data)
    for k, v in pairs(decoded) do
      print(("Message key %s contains value %s"):format(k, v))
      state.positions[message.sender.session_id] = v
    end
    -- PONG message back to sender
    -- dispatcher.broadcast_message(1, message.data, {message.sender})
  end
  -- dispatcher.broadcast_message(OP_LOOP, nk.json_encode(state.presences))
  dispatcher.broadcast_message(OP_LOOP, nk.json_encode(state.positions))
  -- M.match_terminate(context, dispatcher, tick, state, 2)
  return state
end

function M.match_terminate(context, dispatcher, tick, state, grace_seconds)
  local message = "Server shutting down in " .. grace_seconds .. " seconds"
  dispatcher.broadcast_message(OP_TERMINATE, message)
  print("terminated!")
  return nil
end


function table.val_to_str ( v )
  if "string" == type( v ) then
    v = string.gsub( v, "\n", "\\n" )
    if string.match( string.gsub(v,"[^'\"]",""), '^"+$' ) then
      return "'" .. v .. "'"
    end
    return '"' .. string.gsub(v,'"', '\\"' ) .. '"'
  else
    return "table" == type( v ) and table.tostring( v ) or
      tostring( v )
  end
end

function table.key_to_str ( k )
  if "string" == type( k ) and string.match( k, "^[_%a][_%a%d]*$" ) then
    return k
  else
    return "[" .. table.val_to_str( k ) .. "]"
  end
end

function table.tostring( tbl )
  local result, done = {}, {}
  for k, v in ipairs( tbl ) do
    table.insert( result, table.val_to_str( v ) )
    done[ k ] = true
  end
  for k, v in pairs( tbl ) do
    if not done[ k ] then
      table.insert( result,
        table.key_to_str( k ) .. "=" .. table.val_to_str( v ) )
    end
  end
  return "{" .. table.concat( result, "," ) .. "}"
end

return M

