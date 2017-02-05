def run():
    import datetime
    import calendar
    import requests
    import json
    from unidecode import unidecode

    def add_months(sourcedate,months):
        month = sourcedate.month - 1 + months
        year = int(sourcedate.year + month / 12 )
        month = month % 12 + 1
        day = min(sourcedate.day,calendar.monthrange(year,month)[1])
        return datetime.date(year,month,day)

    start = datetime.datetime.now()
    end = add_months(start, 1)
    time = str(start).replace('.', ' ').split(' ')

    start = time[0] + 'T' + time[1] + 'Z'
    end   = str(end)+ 'T' + time[1] + 'Z'

    TIME = [start, end]
    API_KEY = "__API__KEY__"
    MONTREAL = ["45.531507","-73.694145","25"]
    API_CALL = "https://www.eventbriteapi.com/v3/events/search/?token="+API_KEY+"&location.latitude="+MONTREAL[0]+"&location.longitude="+MONTREAL[1]+"&location.within="+MONTREAL[2]+"km&start_date.range_start="+TIME[0]+"&start_date.range_end="+TIME[1]+"&page="
    VENUE_CALL = ["https://www.eventbriteapi.com/v3/venues/",'', "/?token=" + API_KEY]
    VENUES = {}

    tempTypes = json.loads(requests.get("https://www.eventbriteapi.com/v3/categories/?token="+API_KEY).content)["categories"]
    TYPES = {}
    for i in tempTypes:
        TYPES[i["id"]] = i["name"]

    def getEvents():
        page = 1
        api_answer = json.loads(requests.get(API_CALL+str(page)).content)
        page_total = int(api_answer['pagination']['page_count'])
        events = api_answer['events']

        for i in range(2, page_total + 1, 1):
            page += 1
            api_answer = json.loads(requests.get(API_CALL+str(page)).content)
            events += api_answer['events']

        return events

    events = getEvents()

    # TODO VENUE_CALL
    # TODO https://www.eventbriteapi.com/v3/categories/?token=GGMVTKON5EVCRXEPTINS
    output = []
    for event in events:
        location_call = VENUE_CALL

        if event["venue_id"] == None or event["venue_id"] == "None":
            continue
        else:
            try:
                location = VENUES[event["venue_id"]]
            except:
                location_call[1] = event["venue_id"]
                location_call = ''.join(location_call)
                location = json.loads(requests.get(location_call).content)
                VENUES[event["venue_id"]] = location                

        outEvent = {}
        outEvent["origin"] = "Eventbrite"
        outEvent["id"] = event["id"]
        outEvent["name"] = unidecode(event["name"]["text"])
        try:
            if event["description"]["text"] == None or event["description"]["text"] == "None":
                outEvent["description"]["text"] = ""
            else:
                outEvent["description"] = unidecode(event["description"]["text"])
        except:
            outEvent["description"] = ""

        outEvent["expected"] = int(int(event["capacity"])/2.5)
        outEvent["start"] = event["start"]["local"].replace('T', ' ')
        outEvent["end"] = event["end"]["local"].replace('T', ' ')
        try:
            outEvent["logo"] = event["logo"]["original"]["url"]
        except:
            outEvent["logo"] = ""
        try:
            outEvent["type"] = [TYPES[event["category_id"]]]
        except:
            outEvent["type"] = []

        outEvent["location"] = {}
        outEvent["location"]["longitude"] = location["longitude"]
        outEvent["location"]["latitude"] = location["latitude"]

        output.append(outEvent)

    return output