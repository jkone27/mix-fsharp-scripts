import csv

with open('test.csv') as csv_file:
    csv_reader = csv.reader(csv_file, delimiter=',')

    for row in csv_reader:
        res = row[0]
        print(res)