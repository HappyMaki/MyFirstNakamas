local nk = require("nakama")

local M = {}


function M.match_init(context, setupstate)
  print("match_init")
  local gamestate = {
    presences = {},
    positions = {}
  }
  local tickrate = 1 -- per sec
  local label = setupstate.initialstate.label
  return gamestate, tickrate, label
end

function M.match_join_attempt(context, dispatcher, tick, state, presence, metadata)
  print("someone match_join_attempt!")
  -- if state.presences[presence.user_id] ~= nil then
  --   local acceptuser = false
  --   return state, acceptuser
  -- end
  local acceptuser = true
  return state, acceptuser
end

function M.match_join(context, dispatcher, tick, state, presences)
  print("someone match_join!")
  for _, presence in ipairs(presences) do
    -- presence.data = {0, 0, 0}
    state.presences[presence.user_id] = presence
    
  end
  return state
end

function M.match_leave(context, dispatcher, tick, state, presences)
  print("someone match_leave")
  for _, presence in ipairs(presences) do
    state.presences[presence.user_id] = nil
  end
  return state
end

function M.match_loop(context, dispatcher, tick, state, messages)
  for _, presence in pairs(state.presences) do
    print(("Presence %s named %s"):format(presence.user_id, presence.username))
  end
  for _, message in ipairs(messages) do
    -- print(("Received %s from %s"):format(message.data, message.sender.username))
    local decoded = nk.json_decode(message.data)
    for k, v in pairs(decoded) do
      -- print(("Message key %s contains value %s"):format(k, v))
      if state.presences[message.sender.user_id] ~= nil then
        state.presences[message.sender.user_id].data = nk.json_decode(decoded.payload)
      end
    end
    -- dispatcher.broadcast_message(1, message.data, {message.sender})
  end
  dispatcher.broadcast_message(1, nk.json_encode(state.presences))
  
  return state
end

function M.match_terminate(context, dispatcher, tick, state, grace_seconds)
  print("someone match_terminate")
  local message = "Server shutting down in " .. grace_seconds .. " seconds"
  dispatcher.broadcast_message(2, message)
  return nil
end

return M

