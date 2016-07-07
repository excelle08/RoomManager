/**
 * Created by Excelle on 7/7/16.
 */


$(document).ready(function() {
    getApi('/api/user', {}, function(err, r) {
        if (err || r.privilege == 0) {
            location.assign('/login.html');
        } else {
            indexView(r);
        }
    });
});

function indexView(user) {
    var vm = new Vue({
        el: "#header-nav",
        data: {
            user: user
        },
        methods: {
            isAdmin: function() {
                return this.user.privilege == 2;
            },

            logout: function() {
                deleteApi('/api/user', {}, function(err, r) {
                    location.assign('/login.html');
                });
            }
        }
    });
}
