it = io.lines("queries.run")

request = function()
	e = it()
	if e == nil then
		it = io.lines("queries.run")
		e = it()
	end
	return wrk.format("POST", "/databases/Library/streams/queries",  wrk.headers, e)
end

