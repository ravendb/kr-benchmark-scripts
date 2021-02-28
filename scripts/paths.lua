 -- actually cheaper to read from prepared queries
 -- from disk than do string concat in Lua

file = nil
iter = nil

init = function(args)
    file = args[1]
    iter = io.lines(file)
end


request = function()
    local path = iter()
    if path == nil then -- just read them in a loop
        iter = io.lines(file)
        path = iter()
    end
    return wrk.format('GET', path)
end
