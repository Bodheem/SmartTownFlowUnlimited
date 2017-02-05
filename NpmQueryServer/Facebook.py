def run():
    import datetime
    import calendar
    import time
    import requests
    import json

    start = int(time.time())
    end = start + 2628000

    TIME = [start, end]
    API_KEY = "__APP__ID__|__API__KEY__"
    MONTREAL = [["45.494420958879395","-73.64031370729208","9464.62471360397"],
    ["45.60453626371133","-73.61628111451864","4356"],
    ["45.68081220346607","-73.49732665345073","2925"],
    ["45.57276922293251","-73.54058532044291","3830"],
    ["45.63569957373812","-73.54539184423629","5015"],
    ["45.476556196556515","-73.8530090264976","6162"],
    ["45.44573327211074","-73.92030028626323","5230"],
    ["45.47318575992124","-73.75756530091166","4967"],
    ["45.45247722670899","-73.91824034973979","5978"]]
    DOMAIN = "http://localhost:3000/events?"
    TAIL = "&since="+str(TIME[0])+"&until="+str(TIME[1])+"&sort=time&accessToken=" + API_KEY
    API_CALL = [DOMAIN+"lat="+coord[0]+"&lng="+coord[1]+"&distance="+coord[2]+TAIL for coord in MONTREAL]


    def getEvents():
        events = []
        for i in range(len(API_CALL)):
            api_answer = json.loads(requests.get(API_CALL[i]).content)
            events += api_answer['events']

        return events

    events = getEvents()

    # TODO VENUE_CALL
    # TODO https://www.eventbriteapi.com/v3/categories/?token=GGMVTKON5EVCRXEPTINS

    output = []
    for event in events:

        try:
            location = event["venue"]["location"]
            float(location["longitude"])
            float(location["latitude"])
        except:
            continue

        outEvent = {}

        try:
            outEvent["start"] = event["startTime"].replace('T', ' ')[:-5]
            outEvent["end"] = event["endTime"].replace('T', ' ')[:-5]
        except:
            continue

        outEvent["origin"] = "Facebook"
        outEvent["id"] = event["id"]
        outEvent["name"] = event["name"]
        try:
            if event["description"] == None or event["description"] == "None":
                outEvent["description"]["text"] = ""
            else:
                outEvent["description"] = event["description"]
        except:
            outEvent["description"] = ""

        outEvent["expected"] = int(int(event["stats"]["attending"])*0.8 + int(event["stats"]["maybe"])*0.35 + int(event["stats"]["noreply"])*0.075)
        try:
            outEvent["logo"] = event["venue"]["profilePicture"]
            outEvent["logo"] += ""
        except:
            outEvent["logo"] = ""
        try:
            outEvent["type"] = [event["category"] + ""]
        except:
            outEvent["type"] = []

        outEvent["location"] = {}
        outEvent["location"]["longitude"] = str(location["longitude"])
        outEvent["location"]["latitude"] = str(location["latitude"])

        output.append(outEvent)

    return output
