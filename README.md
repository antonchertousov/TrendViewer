<details>
    <summary>
    Expand to see the user story, interface mockup and workflow
  </summary>
  
  # User story
As a quality engineer I want to have a possibility to load manufacturing data results with points of X, Y and Z coordinates to review and analyze the data for each axis:
-	Check how much variation is there in the values for each axis
-	Check whether any exceptional values (outliers) that differ significantly from other values
-	Define the trend for values change over time (for example values are getting bigger)
Additional features:
-	Visual representation for the values on the measurements scale
-	The median line for the set of values
-	Optional upper and lower outlier limit lines
-	The list with outlier data values if any
-	Ability to change the number of points to analyze (minimum 7 points, maximum – the size of the data set, default = 20)

# User Interface Mockup
<img src="https://github.com/antonchertousov/TrendViewer/assets/121962913/a09b2258-8027-4e77-9270-4a72fe358727">
<br><br>
On application startup no controls are visible except the file path text box, open file button and prompt for select the valid file to start analysis.

Once the valid data file is loaded all the information is shown:
- X axis data with statistic lines (median, upper, lower outlier limits)<br>
-	Outlier table for each axis. If no outliers show the textbox with “No outliers for the axis” text<br>
-	Maximum variation value for the data set on each axis<br>
-	Trend value for each axis (Positive, Negative, Flat)<br>
-	Input textbox for number of points to analyze (default value = 20)<br>
-	The “Recalculate” button to update the view after changing the number of points<br>

# Workflow

•	User press the “Open file” button to select the valid file to analyze<br>
•	If the file is not valid the warning popup appears with error definition<br>
•	The XYZ coordinates data is shown on the screen (see the details in UI Mockup section)<br>
•	User can change the number of data points to analyze and press “Recalculate” button to update the results<br>

# Acceptance criteria

1.	It should be possible to open the valid manufacturing data results file (json format specified)
2.	If file contains invalid data (duplicate ids, invalid format, invalid values) show the warning popup
3.	If file contains less than 7 points show the warning that we need at least 7 points data set
4.	Initially show not more than 20 points of data even if the data set is bigger
5.	If measurement number are mixed in file order them by id before the taking data to analysis
6.	The data from file is available as dots on the graph for each axis (on X – the measurement number, on Y – measured value)
7.	Automatically scale the representation to show all points for each axis
8.	By pointing and left click on the point the measurement number and value should be shown in tooltip
9.	The median line with the value is visible
10.	The outlier limit lines are shown if they are within the max and min values range
11.	If outliers are present, they are shown in the table with “id” and “value” 
12.	The maximum variation value is shown for each axis
13.	The trend value is shown for each axis (possible values: negative, positive, flat)
14.	It should be possible to limit the number of points to analyze: set the value in range of (7-20) and press “Recalculate” button to update the results
15.	The data files with more than 20 data points are allowed
16.	Optional: it should be possible to resize the data graph on vertical axis

</details>

<details>
  <summary>Expand to see the technical details</summary>
  
# Description

The application contains one window to display all the data read from file in json format.
Json data format description:
- “id”: “1” – measurement number (integer)
- “x”: “1.1” – x coordinate value (float)
- “y”: “1.1” – y coordinate value (float)
- “z”: “1.1” – z coordinate value (float)

Initial validation should decline reading the file with incorrect data, duplicate Ids, minimum data points (7).
Expected that measurement Id are increasing from 1 with step=1, but make sure Id’s are in a right order.
All the data from json file should be loaded in memory. In case of changing number of points to show - use data from memory.

# Principles and considerations

Use the MVVM, dependency injection and observer patterns to organize the data flow. Consider to use providers for the measurement data and data processing using the isolated services (use interfaces).

# Components diagram 
![image](https://github.com/antonchertousov/TrendViewer/assets/121962913/4dba3e15-b57e-4b65-8f4a-f879d911b673)

# Data classes
![image](https://github.com/antonchertousov/TrendViewer/assets/121962913/fafc27d1-c5bf-40c6-94ac-85b82368e745)

# Sequence diagram
![image](https://github.com/antonchertousov/TrendViewer/assets/121962913/7cf9823c-37d5-49b2-867d-4baa4ea1be11)

# Scope
- Backend (Data reading, data processing, populating the data model for visual representation)
- Frontend (Preparing the data for the view, UX design, interaction with backend)

# Data processing
## Calculating the maximum variation for the values
Find the minimum and maximum values in the sequence and take the absolute value for these.

## Calculating the trend for the values
Find the slope for the sequence of values (use the slope math calculation). If the slope value greater than 0 assign the trend as positive, if lower than 0 – negative. The ideal flat trend should have the zero slope value, but consider to use the tolerance value i.e. define trend as flat if the absolute slope value not greater than 1/1000.

## Calculating the median value for the data sequence
Order a data set x1 ≤ x2 ≤ x3 ≤ ... ≤ xn from lowest to highest value, the median is the numeric value separating the upper half of the ordered sample data from the lower half. If n is odd the median is the center value. If n is even the median is the average of the 2 center values.

## Calculating the lower and upper outlier limits
1. Calculate the quartiles on the data set. The median is the second quartile Q2. It divides the ordered data set into higher and lower halves.  The first quartile, Q1, is the median of the lower half not including Q2. The third quartile, Q3, is the median of the higher half not including Q2.
2. Calculate the range from Q1 to Q3 is the interquartile range IQR=Q3−Q1
3. Potential outliers are values that lie above the Upper Limit or below the Lower Limit of the sample set:
Upper Limit=Q3+1.5×IQR
Lower Limit=Q1−1.5×IQR

# Logging
The application should have the appropriate level of logging for actions and calculations. Use the log4net component with these levels:
-	Error level – for any error during file operations or calculation
-	Info level – for logging the common actions
-	Debug level – for logging the calculation results

# Acceptance criteria
-	Source Code
-	Executable and library files required to run the application
-	Valid and invalid json data files to show functionality
-	Log file created in the application executable folder
-	Unit tests for basic components, the total test coverage ~80%
-	Technical documentation (take from the technical description of this document)

# Test data
The samples are located in TestData folder.
</details>
