var Controllers;
(function (Controllers) {
    var RuleController = (function () {
        function RuleController(defaultVersion, defaultRule) {
            this.defaultVersion = defaultVersion;
            this.defaultRule = defaultRule;
            var hash = {
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
        RuleController.prototype.openRequestedPage = function (hash) {
            if (!hash.version) {
                this.handleVersionError();
                return;
            }
            var requestedVersion = hash.version;
            if (!(new RegExp(/^([a-zA-Z0-9-\.]+)$/)).test(requestedVersion)) {
                this.handleVersionError();
                return;
            }
            //display page:
            var self = this;
            this.getRulesJson(requestedVersion, function () {
                self.displayMenu(hash);
                if (!hash.ruleId) {
                    self.handleRuleIdError(false);
                    return;
                }
                self.displayRulePage(hash);
            });
        };
        RuleController.prototype.parseHash = function () {
            var hash = (location.hash || '').replace(/^#/, '').split('&'), parsed = {};
            for (var i = 0, el; i < hash.length; i++) {
                el = hash[i].split('=');
                parsed[el[0]] = el[1];
            }
            return parsed;
        };
        RuleController.prototype.displayMenu = function (hash) {
            var menu = document.getElementById("rule-menu");
            var currentVersion = menu.getAttribute("data-version");
            if (currentVersion == this.currentVersion) {
                return;
            }
            var listItems = '';
            for (var i = 0; i < this.currentRules.length; i++) {
                listItems += '<li><a href="#version=' + this.currentVersion + '&ruleId=' + this.currentRules[i].Key + '" title="' + this.currentRules[i].Title + '">' + this.currentRules[i].Title + '</a></li>';
            }
            document.getElementById("rule-count").innerHTML = '(count: ' + this.currentRules.length + ')';
            menu.innerHTML = listItems;
            menu.setAttribute("data-version", this.currentVersion);
        };
        RuleController.prototype.displayRulePage = function (hash) {
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
        };
        RuleController.prototype.handleRuleIdError = function (hasMenuIssueToo) {
            if (!hasMenuIssueToo) {
                var ruleId = document.getElementById("rule-id");
                ruleId.innerHTML = "ERROR: couldn't find rule";
                ruleId.style.visibility = 'visible';
            }
            document.getElementById("rule-title").innerHTML = "";
            var tags = document.getElementById("rule-tags");
            tags.innerHTML = "";
            tags.style.visibility = 'hidden';
            document.getElementById("rule-description").innerHTML = "";
        };
        RuleController.prototype.handleVersionError = function () {
            this.handleRuleIdError(true);
            var menu = document.getElementById("rule-menu");
            menu.innerHTML = "";
            menu.setAttribute("data-version", "");
            var ruleId = document.getElementById("rule-id");
            ruleId.innerHTML = "ERROR: couldn't find version";
            ruleId.style.visibility = 'visible';
        };
        RuleController.prototype.hashChanged = function () {
            var hash = {
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
        };
        RuleController.prototype.getRulesJson = function (version, callback) {
            if (this.currentVersion != version) {
                var self = this;
                //load file
                this.loadJSON('../rules/' + version + '/rules.json', function (jsonString) {
                    self.currentVersion = version;
                    self.currentRules = JSON.parse(jsonString);
                    callback();
                });
                return;
            }
            callback();
        };
        RuleController.prototype.loadJSON = function (path, callback) {
            var self = this;
            var xobj = new XMLHttpRequest();
            xobj.overrideMimeType("application/json");
            xobj.open('GET', path, true);
            xobj.onload = function () {
                if (this.status == 200) {
                    callback(xobj.responseText);
                }
                else {
                    self.handleVersionError();
                }
            };
            xobj.send(null);
        };
        return RuleController;
    })();
    Controllers.RuleController = RuleController;
})(Controllers || (Controllers = {}));
//# sourceMappingURL=RuleController.js.map