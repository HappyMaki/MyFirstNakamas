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
  local label = setupstate.initialstate.label
  return gamestate, tickrate, label
end

function M.match_join_attempt(context, dispatcher, tick, state, presence, metadata)
  local acceptuser = true
  print("someone joined attempt!")
  return state, acceptuser
end

function M.match_join(context, dispatcher, tick, state, presences)
  for _, presence in ipairs(presences) do
    state.presences[presence.session_id] = presence
  end
  return state
end

function M.match_leave(context, dispatcher, tick, state, presences)
  for _, presence in ipairs(presences) do
    state.presences[presence.session_id] = nil
  end
  return state
end

function M.match_loop(context, dispatcher, tick, state, messages)
  for _, message in ipairs(messages) do
    local decoded = nk.json_decode(message.data)
    for k, v in pairs(decoded) do
      print(("Message key %s contains value %s"):format(k, v))
      state.positions[message.sender.session_id] = v
    end
  end
  dispatcher.broadcast_message(OP_LOOP, nk.json_encode(state.positions))
  return state
end

function M.match_terminate(context, dispatcher, tick, state, grace_seconds)
  local message = "Server shutting down in " .. grace_seconds .. " seconds"
  dispatcher.broadcast_message(OP_TERMINATE, message)
  print("terminated!")
  return nil
end

return M

