Required Unity version: 5.0.0 Personal

PHP Pages:
	501.php-
		Main login script. Returns user ID, name, and whether the user is already logged into another instance.
		in-
			<user_email>
			<user_pass>
		out-
			<user_id>&<user_name>&<loggedIn>

	502.php
		Reserves the instance. Makes it impossible for the user to log into another instance.
		in-
			<user_id>
		out-
			N/A
	
	503.php
		Unreserves the instance. Makes it possible for the user to log into another instance.
		in-
			<user_id>
		out-
			N/A	