 -- actually cheaper to read from prepared queries
 -- from disk than do string concat in Lua

file = nil
iter = nil

init = function(args)
    file = args[1]
    iter = io.lines(file)
end


request = function()
    wrk.path = iter()
    if wrk.path = nil then -- just read them in a loop
        iter = io.lines(file)
        wrk.path = iter()
    end
end
