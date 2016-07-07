

$(document).ready(loginView);


function loginView() {
    var vm = new Vue({
        el: "#login-form",
        data: {
            username: '',
            password: '',
            remember: false
        },
        methods: {
            login: function() {
                if (this.username == "") {
                    return showError("Please enter username");
                }
                if (this.password == "") {
                    return showError("Please enter password");
                }
    
                postApi('/api/user', {
                    username: this.username,
                    password: this.password,
                    remember: this.remember
                }, function(err, r) {
                    if (err) {
                        return showError(err.message);
                    } else {
                        return location.assign('/');
                    }
                });
            }
        }
    
    });
}