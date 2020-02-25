# Bulk print

Example on how to print labels from addresses found in a CSV file. This example use Python to talk to the Edipost API
and a Bash script for automation.

The script will generate `all-labels.pdf` which contains all the shipping labels and the report `shipments.csv`


## Usage

1. Create addresses.csv with the following format:
    * Name
    * Address
    * Zip code
    * City
    * Email
    * Cellphone
    
    For example:
    ```
    Donald Duck;Andebyveien 1;1337;Sandvika;donald@andeby.no;98765111
    Dolly Duck;Andebyveien 2;1337;Sandvika;dolly@andeby.no;98765222
    Anton Duck;Andebyveien 3;1337;Sandvika;anton@andeby.no;98765333
    ```
   
2. Enter the correct `API_KEY` and `SERVICE` in create-all.sh

3. Run the script
    ```
    $ cat addresses.csv | ./create-all.sh 
    Creating shipment #1
    Creating shipment #2
    Creating shipment #3
    ```
   
4. Check `shipments.csv` for errors

    Example:
    ```
    1;Donald Duck;Andebyveien 1;1337;Sandvika;donald@andeby.no;98765111;70209000003482348
    2;Dolly Duck;Andebyveien 2;1337;Sandvika;dolly@andeby.no;98765222;70209000003482355
    3;Anton Duck;Andebyveien 3;1337;Sandvika;anton@andeby.no;98765333;70209000003482362
    ```
   
5. Print `all-labels.pdf`