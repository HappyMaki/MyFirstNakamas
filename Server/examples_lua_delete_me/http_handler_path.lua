local nk = require("nakama")

local function http_handler(context, payload)
  local message = nk.json_decode(payload)
  nk.logger_info(("Message: %q"):format(message))
  return nk.json_encode({["context"] = context})
end

nk.register_rpc(http_handler, "http_handler_path")