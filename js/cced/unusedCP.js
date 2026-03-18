
function getUnusedSkillPoints(playerModel) {
    var unusedSkillPoints = 0;

    for (var i = 0; i < playerModel.skillPoints.length; ++i) {
        unusedSkillPoints += playerModel.skillPoints[i];
    }

    return unusedSkillPoints + "";
}

sc.PlayerModel.inject({
    onVarAccess(a, b) {
        // We need to run our code before parent() to avoid 
        // an "Unsupported var access" error that will be thrown by parent()
        if (b[0] == "player") {
            switch (b[1]) {
                case "unusedCP":
                    return getUnusedSkillPoints(this);
                default:
                    break;
            }
        }

        return this.parent(a, b);
    } 
});
