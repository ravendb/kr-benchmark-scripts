 -- actually cheaper to read from prepared queries
 -- from disk than do string concat in Lua

 file = nil
 iter = nil
 path = nil

 init = function(args)
     file = args[1]
     path = args[2]
     iter = io.lines(file)
 end
 
 
 request = function()
    wrk.path = path   
    wrk.body = iter()
    if wrk.body = nil then -- just read them in a loop
         iter = io.lines(file)
         wrk.body = iter()
    end
 end
 