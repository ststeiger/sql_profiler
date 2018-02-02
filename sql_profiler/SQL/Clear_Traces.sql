
DECLARE @trace_id AS integer; 
DECLARE @trace_iterator AS CURSOR; 

SET @trace_iterator = CURSOR FOR 
( 
	SELECT id FROM sys.traces WHERE is_default <> 1 
); 


OPEN @trace_iterator; 
FETCH NEXT FROM @trace_iterator INTO @trace_id; 

WHILE @@FETCH_STATUS = 0 
BEGIN 
	PRINT 'sp_trace_setstatus ' + CAST(@trace_id AS varchar(20)) + ', 0'; 
	PRINT 'sp_trace_setstatus ' + CAST(@trace_id AS varchar(20)) + ', 2'; 

	-- 0: Stops the specified trace.
	-- 1: Starts the specified trace.
	-- 2: Closes the specified trace and deletes its definition from the server.

	EXEC sp_trace_setstatus @trace_id, 0; 
	EXEC sp_trace_setstatus @trace_id, 2; 

	FETCH NEXT FROM @trace_iterator INTO @trace_id; 
END 
 
CLOSE @trace_iterator; 
DEALLOCATE @trace_iterator; 
