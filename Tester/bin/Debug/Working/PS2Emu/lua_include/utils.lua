-- utility classes/functions

-- Stack
-- ex:
--		my_stack = Stack.new()
--  	my_stack:push( val )
--		print( my_stack:pop( val ) )
Stack = {}

function Stack.new()
   local obj = { buff = {} }
   return setmetatable(obj, {__index = Stack})
end

function Stack:push(x)
  table.insert(self.buff, x)
end

function Stack:pop()
  return table.remove(self.buff)
end

function Stack:top()
  return self.buff[#self.buff]
end

function Stack:isEmpty()
  return #self.buff == 0
end


-- Queue
-- ex:
--		my_queue = Queue.new()
--		my_queue:enqueue( val )
--		print( my_queue:dequeue(val) )
Queue = {}

function Queue.new()
  local obj = { buff = {} }
  return setmetatable(obj, {__index = Queue})
end

function Queue:enqueue(x)
  table.insert(self.buff, x)
end

function Queue:dequeue()
  return table.remove(self.buff, 1)
end

function Queue:top()
  if #self.buff > 0 then
    return self.buff[1]
  end
end

function Queue:isEmpty()
  return #self.buff == 0
end
