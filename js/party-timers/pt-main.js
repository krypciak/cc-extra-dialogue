
// Todo: Break down into separate files

var printDebugLog = false;

function logDebug(message) {
    if (printDebugLog) {
        console.log(message);
    }
}

function logVarChange(varName) {
    logDebug("Changed " + varName + " to " + ig.vars.get(varName));
}

function loggedSet(varName, value) {
    ig.vars.set(varName, value);
    logVarChange(varName);
}

// Returns formatted variable names
var iptVars = {
	// Parameter glossary
	// memberName: Apollo, Emilie, Glasses etc.
	// partyName: Apollo-Emilie, Glasses-Joern etc.
	// name: Either of the above

	prefix: "ccipt.vars.", // Mod variables prefix 

    varsRoot: function() {
        return this.prefix.slice(0, -1);
    },

    // ### Universal ###

	// Accepts both memberName and partyName
	downtimeStart: function(name) {
		return this.prefix + name + ".downtimeStart";
	},

    downtimeEnd: function(name) {
		return this.prefix + name + ".downtimeEnd";
	},

    lastDowntime: function(name) {
		return this.prefix + name + ".lastDowntime";
	},

    maxDowntime: function(name) {
		return this.prefix + name + ".maxDowntime";
	},
	
	// Accepts both memberName and partyName
	trackDowntime: function(name) {
		return this.prefix + name + ".trackDowntime";
	},

    totalPartyTime: function(name) {
		return this.prefix + name + ".totalPartyTime";
	},

    // ### Party only ###
	// Returns alphabetically sorted party combo name 
	partyName: function(memberNameA, memberNameB) {
		if (memberNameA.localeCompare(memberNameB) <= 0) {
			return memberNameA + "-" + memberNameB;
		}
		else {
			return memberNameB + "-" + memberNameA;
		}
	},

	partyFormedAt: function(partyName) {
		return this.prefix + partyName + ".formedAt";
	},

	partyDisbandedAt: function(partyName) {
		return this.prefix +partyName + ".disbandedAt";
	},

    // ### Member only ###   
	// Returns current playtime
	memberJoinedAt: function(memberName) {
		return this.prefix + memberName + ".joinedAt";
	},

	memberLeftAt: function(memberName) {
		return this.prefix + memberName + ".leftAt";
	},
}

var helpers = {
    now: function() {
    	return ig.vars.get("stat.player.playtime");
	},

    currentParty: function() {
        return sc.party["currentParty"].slice();
    },

    currentPartyWithoutMember(memberName) {
        var party = this.currentParty();
        var i = party.indexOf(memberName);
        if (i > -1) {
            party.splice(i, 1);
        }
        return party;
    },

    updateJoinedAt: function(memberName, currentTime) {
        loggedSet(iptVars.memberJoinedAt(memberName), currentTime);
    },

    updateLeftAt: function(memberName, currentTime) {
        loggedSet(iptVars.memberLeftAt(memberName), currentTime);
    },

    updatePartyFormedAt: function(memberName, currentTime) {
        helpers.currentPartyWithoutMember(memberName).forEach(otherMember => {
            var partyName = iptVars.partyName(memberName, otherMember);
            loggedSet(iptVars.partyFormedAt(partyName), currentTime);
        });
    },

    resetPartyFormedAt: function(memberName) {
        helpers.currentPartyWithoutMember(memberName).forEach(otherMember => {
            var partyName = iptVars.partyName(memberName, otherMember);
            loggedSet(iptVars.partyFormedAt(partyName), null);
        });
    },

    updatePartyDisbandedAt: function(memberName, currentTime) {
        helpers.currentPartyWithoutMember(memberName).forEach(otherMember => {
            var partyName = iptVars.partyName(memberName, otherMember);
            loggedSet(iptVars.partyDisbandedAt(partyName), currentTime);
        });
    },

    resetPartyDisbandedAt: function(memberName) {
        helpers.currentPartyWithoutMember(memberName).forEach(otherMember => {
            var partyName = iptVars.partyName(memberName, otherMember);
            loggedSet(iptVars.partyDisbandedAt(partyName), null);
        });
    },

    updateTotalPartyTime: function(memberName) {
        
        var joinedAt = ig.vars.get(iptVars.memberJoinedAt(memberName));
        var leftAt = ig.vars.get(iptVars.memberLeftAt(memberName));

        if (joinedAt > 0 && leftAt > 0) {
            var delta = leftAt - joinedAt;

            var totalPartyTime = ig.vars.get(iptVars.totalPartyTime(memberName));
            loggedSet(iptVars.totalPartyTime(memberName), totalPartyTime + delta);
        }

        helpers.currentPartyWithoutMember(memberName).forEach(otherMember => {
            var partyName = iptVars.partyName(memberName, otherMember);

            var formedAt = ig.vars.get(iptVars.partyFormedAt(partyName));
            var disbandedAt = ig.vars.get(iptVars.partyDisbandedAt(partyName));
            if (formedAt > 0 && disbandedAt > 0) {
                var delta = disbandedAt - formedAt;

                var totalPartyTime = ig.vars.get(iptVars.totalPartyTime(partyName));
                loggedSet(iptVars.totalPartyTime(partyName), totalPartyTime + delta);
            }
        });
    },

    disableDowntime: function(name) {
        loggedSet(iptVars.downtimeStart(name), null);
        loggedSet(iptVars.downtimeEnd(name), null);
        loggedSet(iptVars.lastDowntime(name), null);
        loggedSet(iptVars.maxDowntime(name), null);
        loggedSet(iptVars.trackDowntime(name), null);
    },

    startDowntime: function(memberName, currentTime) {
        var root = ig.vars.get(iptVars.varsRoot());
        if (root == null) {
            return;
        }

        Object.keys(root).forEach(element => {
            
            if (element.includes(memberName)) {
                
                var trackDowntime = ig.vars.get(iptVars.trackDowntime(element));
                if (trackDowntime > 0) {
                    
                    var cancel = false;
                    helpers.currentPartyWithoutMember(memberName).forEach(otherMember =>{
                        if (element.includes(otherMember)) {
                            cancel = true;
                            return;
                        }
                    });

                    if (cancel) {
                        return;
                    }

                    loggedSet(iptVars.downtimeStart(element), currentTime);
                    loggedSet(iptVars.downtimeEnd(element), null);
                }
                else if (trackDowntime != null) {
                    this.disableDowntime(element);
                }

            }

        });
    },

    endDowntime: function(memberName, currentTime) {
        var root = ig.vars.get(iptVars.varsRoot());
        if (root == null) {
            return;
        }
        
        Object.keys(root).forEach(element => {

            if (element.includes(memberName)) {

                var trackDowntime = ig.vars.get(iptVars.trackDowntime(element));
                if (trackDowntime > 0) {

                    var downtimeStart = ig.vars.get(iptVars.downtimeStart(element));
                    if (downtimeStart != null) {
                        var downtimeEnd = currentTime;
                        var delta = downtimeEnd - downtimeStart;
                        
                        loggedSet(iptVars.downtimeEnd(element), currentTime);
                        loggedSet(iptVars.lastDowntime(element), delta);
                        if (delta > ig.vars.get(iptVars.maxDowntime(element))) {
                            loggedSet(iptVars.maxDowntime(element), delta);
                        }
                        loggedSet(iptVars.downtimeStart(element), null);
                    }
                    
                }
                else if (trackDowntime != null) {
                    this.disableDowntime(element);
                }

            }

        });
    }

}


function onMemberJoined(memberName) {

    logDebug("Member joined: " + memberName);

    var currentTime = helpers.now();
    helpers.updateJoinedAt(memberName, currentTime);
    helpers.updatePartyFormedAt(memberName, currentTime);
    helpers.endDowntime(memberName, currentTime);
    helpers.resetPartyDisbandedAt(memberName);
}

function onMemberLeft(memberName) {

    logDebug("Member left: " + memberName);

    var currentTime = helpers.now();
    helpers.updateLeftAt(memberName, currentTime);
    helpers.updatePartyDisbandedAt(memberName, currentTime);
    helpers.updateTotalPartyTime(memberName);
    helpers.startDowntime(memberName, currentTime);
    helpers.resetPartyFormedAt(memberName);
}

// ..........

sc.PartyModel.inject({
    addPartyMember(a, b, c, d, i) {
        this.parent(a, b, c, d, i);
        onMemberJoined(a);
    },

    removePartyMember(a, b, c) {
        this.parent(a, b, c);
        onMemberLeft(a);
    }
});