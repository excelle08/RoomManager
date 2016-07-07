/**
 * Created by Excelle on 7/8/16.
 */

$(document).ready(function () {
    var args = getArgs();
    if (!args.group) {
        location.assign('/');
    }

    deleteApi('/api/group/' + args.group, {}, function (err, r) {
        if (err) {
            alert('Failed to check out: ' + err.message);
        } else {
            getApi('/api/group/' + args.group, {}, function (err2, group) {
                if (err2) {
                    alert('Error retrieving group info. ' + err2.message);
                } else {
                    getApi('/api/customer/' + group.leader_Id, {}, function (err3, customer) {
                        if (err3) {
                            alert('Cannot load specific user info - ' + err3.message);
                        } else {
                            groupCheckoutView(customer, r);
                        }
                    })
                }
            });
        }
    });
});


function groupCheckoutView(leader, consumptions) {
    var vm = new Vue({
        el: '#checkout-form',
        data: {
            leader: leader,
            consumptions: consumptions
        },
        methods: {
            getSum: function () {
                var sum = 0.0;
                for(var i in this.consumptions) {
                    sum += this.consumptions[i].price;
                }
                return sum;
            },

            proceed: function () {
                
            }
        }
    });
}