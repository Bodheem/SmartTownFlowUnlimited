import Eventbrite
import Facebook
import json
import time, os, stat
events = []


try:
    if (time.time() - os.stat("events.json")[stat.ST_MTIME]) > 3600 :
        os.remove("events.json")
    events = json.loads(open("events.json", 'r').read())

except:
    fb = Facebook.run()
    eb = Eventbrite.run()
    temp = fb + eb
    def farEnough(posPair, endPair):
        if endPair[0][:-5] != endPair[1][:-5]:
            return True
        from math import sin, cos, sqrt, atan2, radians

        R = 6373.0

        lat1 = radians(round(posPair[0][0], 5))
        lon1 = radians(round(posPair[0][1], 5))
        lat2 = radians(round(posPair[1][0], 5))
        lon2 = radians(round(posPair[1][1], 5))

        dlon = lon2 - lon1
        dlat = lat2 - lat1

        a = sin(dlat / 2)**2 + cos(lat1) * cos(lat2) * sin(dlon / 2)**2
        c = 2 * atan2(sqrt(a), sqrt(1 - a))

        distance = R * c * 1000

        if distance < 50:
            return False

        return True
    events = []
    while(len(temp) != 0):
        obj = temp.pop()
        invite_obj = int(obj["expected"])
        if invite_obj < 50:
            continue
        lat_obj = float(obj["location"]["longitude"])
        lon_obj = float(obj["location"]["latitude"])
        end_obj = obj["end"]
        discard = False
        for event in events:      
            lat_event = float(event["location"]["longitude"])
            lon_event = float(event["location"]["latitude"])
            end_event = event["end"]
            if farEnough([[lat_obj, lon_obj], [lat_event, lon_event]], [end_obj, end_event]) == False:
                discard = True
                break
        if discard == True:
            continue
        events.append(obj)
    open("events.json", 'w').write(json.dumps(events))

import time
import BaseHTTPServer

PORT = 8000

class MyHandler(BaseHTTPServer.BaseHTTPRequestHandler):
    def do_HEAD(s):
        s.send_response(200)
        s.send_header("Access-Control-Allow-Origin", "*")
        s.end_headers()
    def do_GET(s):
        s.send_response(200)
        s.send_header("Access-Control-Allow-Origin", "*")
        s.end_headers()
        s.wfile.write(json.dumps(events))

server_class = BaseHTTPServer.HTTPServer
httpd = server_class(("", PORT), MyHandler)
print (time.asctime(), "Server Starts - Serving port %s" % (PORT))
httpd.serve_forever()