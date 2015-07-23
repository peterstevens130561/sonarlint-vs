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
            var parsedHash = this.parseHash();
            if (parsedHash.version) {
                hash.version = parsedHash.version;
            }
            if (parsedHash.ruleId) {
                hash.ruleId = parsedHash.ruleId;
            }

            this.openRequestedPage(hash);    
        }

        public openRequestedPage(hash: UrlParams) {
            if (!hash.version) {
                this.handleVersionError();
                return;
            }
            
            var requestedVersion = hash.version;

            if (!(new RegExp(<any>/^([a-zA-Z0-9-\.]+)$/)).test(requestedVersion)) {
                this.handleVersionError();
                return;
            }
            
            //display page:
            var self = this;
            this.getRulesJson(requestedVersion, () => {
                self.displayMenu(hash);

                if (!hash.ruleId)
                {
                    self.handleRuleIdError(false);
                    return;
                }

                self.displayRulePage(hash);
            });            
        }

        private parseHash(): any
        {
            var hash = (location.hash || '').replace(/^#/, '').split('&'),
                parsed = {};

            for (var i = 0, el; i < hash.length; i++) {
                el = hash[i].split('=')
                parsed[el[0]] = el[1];
            }
            return parsed;
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
                    var ruleId = document.getElementById("rule-id");
                    ruleId.innerHTML = 'Rule ID: ' + this.currentRules[i].Key;
                    ruleId.style.visibility = 'visible';

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
            this.handleRuleIdError(false);
        }

        private handleRuleIdError(hasMenuIssueToo: boolean) {
            if (hasMenuIssueToo) {
                var ruleId = document.getElementById("rule-id");
                ruleId.innerHTML = "ERROR: couldn't find rule";
                ruleId.style.visibility = 'visible';
            }

            document.getElementById("rule-title").innerHTML = "";
            var tags = document.getElementById("rule-tags");
            tags.innerHTML = "";
            tags.style.visibility = 'hidden';
            document.getElementById("rule-description").innerHTML = "";
        }
        private handleVersionError() {
            this.handleRuleIdError(true);

            var menu = document.getElementById("rule-menu");
            menu.innerHTML = "";
            menu.setAttribute("data-version", "");

            var ruleId = document.getElementById("rule-id");
            ruleId.innerHTML = "ERROR: couldn't find version";
            ruleId.style.visibility = 'visible';
        }
        
        public hashChanged()
        {
            var hash: UrlParams = {
                version: this.defaultVersion,
                ruleId: this.defaultRule
            };
            var parsedHash = this.parseHash();            
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
