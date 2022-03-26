Vue.component('AccountChange', {
    template: `
        <div>
            <form action="/account_change/validate">
                <div class="mb-3">
                    <label for="old-password" class="form-label">Old password</label>
                    <input type="password" class="form-control" id="old-password" name="old_password">
                </div>
                <div class="mb-3">
                    <label for="password" class="form-label">New password</label>
                    <input type="password" class="form-control" id="password" name="new_password">
                </div>
                <button type="submit" class="btn btn-primary">Submit</button>
            </form>
        </div>
    `
});

