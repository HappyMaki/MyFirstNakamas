local nk = require("nakama")

local M = {}


function M.match_init(context, setupstate)
  print("match_init")
  local gamestate = {
    presences = {},
    positions = {}
  }
  local tickrate = 30 -- per sec
  local label = setupstate.initialstate.label
  return gamestate, tickrate, label
end

function M.match_join_attempt(context, dispatcher, tick, state, presence, metadata)
  -- TODO: don't let same user log in twice. I HAVE NO IDEA WHAT WILL HAPPEN
  print("someone match_join_attempt!")
  local acceptuser = true
  return state, acceptuser
end

function M.match_join(context, dispatcher, tick, state, presences)
  print("someone match_join!")
  for _, presence in ipairs(presences) do
    state.presences[presence.user_id] = presence
    
  end
  return state
end

function M.match_leave(context, dispatcher, tick, state, presences)
  print("someone match_leave")
  for _, presence in ipairs(presences) do
    local user_id = presence.user_id
    local new_objects = {
      {collection = "character", key = "location", user_id = user_id, value = state.presences[presence.user_id]},
    }
    nk.storage_write(new_objects)
    state.presences[presence.user_id] = nil
  end
  return state
end

function M.match_loop(context, dispatcher, tick, state, messages)
  for _, message in ipairs(messages) do
    local decoded = nk.json_decode(message.data)
    for k, v in pairs(decoded) do
      if state.presences[message.sender.user_id] ~= nil then
        state.presences[message.sender.user_id].data = nk.json_decode(decoded.payload)
      end
    end
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

