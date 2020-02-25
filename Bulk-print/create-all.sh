#!/usr/bin/env bash

OLDIFS=$IFS
IFS=";"

API_KEY="8745jh2345nd324kbj567vv3454kj657"
SERVICE="8"

cnt=1

while read name address zip city email phone
do
  echo "Creating shipment #$cnt"
  shipmentnr=$(./create-shipment.py "$API_KEY" "$name" "$address" "" "$zip" "$city" "$email" "$phone" "$SERVICE" -f "label-$cnt.pdf")
  echo "$cnt;$name;$address;$zip;$city;$email;$phone;$shipmentnr" >> shipments.csv
  cnt=$((cnt+1))
done

pdfunite *.pdf all-labels.pdf

IFS=$OLDIFS