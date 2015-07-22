interface Location
{
    parseHash(): UrlParams;
}
interface UrlParams
{
    version: string;
    ruleId: string;
}
interface Rule {
    Key: string;
    Title: string;
    Description: string;
    Tags: string;
}

(<any>location).parseHash = function () {
    var hash = (this.hash || '').replace(/^#/, '').split('&'),
        parsed = {};

    for (var i = 0, el; i < hash.length; i++) {
        el = hash[i].split('=')
        parsed[el[0]] = el[1];
    }
    return parsed;
};

module Controllers {
    export class RuleController {
        defaultVersion: string;
        defaultRule: string;
        
        constructor(defaultVersion: string, defaultRule: string) {
            this.defaultVersion = defaultVersion;
            this.defaultRule = defaultRule;
            var hash: UrlParams = {
                version: this.defaultVersion,
                ruleId: this.defaultRule
            };
            var parsedHash = location.parseHash();
            if (parsedHash.version) {
                hash.version = parsedHash.version;
            }
            if (parsedHash.ruleId) {
                hash.ruleId = parsedHash.ruleId;
            }

            this.openRequestedPage(hash);    
        }

        public openRequestedPage(hash: UrlParams) {
            if (!hash.ruleId || !hash.version)
            {
                this.handleError();
                return;
            }

            //display page:
            var self = this;

            var requestedVersion = hash.version;

            if (!(new RegExp(<any>/^([a-zA-Z0-9-\.]+)$/)).test(requestedVersion))
            {
                this.handleError();
                return;
            }

            this.getRulesJson(requestedVersion,() => {
                self.displayMenu(hash);
                self.displayRulePage(hash);                
            });
        }

        private displayMenu(hash: UrlParams) {
            var menu = document.getElementById("rule-menu");
            var currentVersion = menu.getAttribute("data-version");

            if (currentVersion == this.currentVersion)
            {
                return;
            }

            var listItems = '';
            for (var i = 0; i < this.currentRules.length; i++) {
                listItems += '<li><a href="#version=' + this.currentVersion + '&ruleId=' + this.currentRules[i].Key + '" title="' + this.currentRules[i].Title + '">' + this.currentRules[i].Title + '</a></li>';
            }

            menu.innerHTML = listItems;
            menu.setAttribute("data-version", this.currentVersion);
        }

        private displayRulePage(hash: UrlParams) {
            var doc = document.documentElement;
            var left = (window.pageXOffset || doc.scrollLeft) - (doc.clientLeft || 0);
            var top = (window.pageYOffset || doc.scrollTop) - (doc.clientTop || 0);

            for (var i = 0; i < this.currentRules.length; i++) {
                if (this.currentRules[i].Key == hash.ruleId) {
                    //we have found it
                    document.getElementById("rule-id").innerHTML = this.currentRules[i].Key;
                    document.getElementById("rule-title").innerHTML = this.currentRules[i].Title;
                    var tags = document.getElementById("rule-tags");
                    tags.innerHTML = this.currentRules[i].Tags;
                    if (this.currentRules[i].Tags) {
                        tags.style.visibility = 'visible';
                    }
                    else {
                        tags.style.visibility = 'hidden';
                    }

                    document.getElementById("rule-description").innerHTML = this.currentRules[i].Description;
                    window.scrollTo(left, 0);
                    return;
                }
            }
            this.handleError();
        }

        private handleError()
        {
            document.getElementById("rule-id").innerHTML = "ERROR";
            document.getElementById("rule-title").innerHTML = "";
            var tags = document.getElementById("rule-tags");
            tags.innerHTML = "";
            tags.style.visibility = 'hidden';
            document.getElementById("rule-description").innerHTML = "";

            var menu = document.getElementById("rule-menu");
            menu.innerHTML = "";
            menu.setAttribute("data-version", "");
        }
        
        public hashChanged()
        {
            var hash: UrlParams = {
                version: this.defaultVersion,
                ruleId: this.defaultRule
            };
            var parsedHash = location.parseHash();            
            if (parsedHash.version) {
                hash.version = parsedHash.version;
            }
            if (parsedHash.ruleId) {
                hash.ruleId = parsedHash.ruleId;
            }
            this.openRequestedPage(hash);
        }

        currentVersion: string;
        currentRules: Rule[];
        private getRulesJson(version: string, callback: Function)
        {
            if (this.currentVersion != version)
            {
                var self = this;
                //load file
                this.loadJSON('../rules/'+version+'/rules.json',(jsonString) => {
                    self.currentVersion = version;
                    self.currentRules = JSON.parse(jsonString);
                    callback();
                });   
                return;             
            }

            callback();
        }
        loadJSON(path: string, callback: Function) {
            var xobj = new XMLHttpRequest();
            xobj.overrideMimeType("application/json");
            xobj.open('GET', path, true); 
            xobj.onload = function () {
                callback(xobj.responseText);
            };
            xobj.send(null);
        }
    }
}
