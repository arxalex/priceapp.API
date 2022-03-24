Vue.component('Login', {
    template: `
        <div>
            <form action="/login/validate">
                <div class="mb-3">
                    <label for="username" class="form-label">Username</label>
                    <input type="text" class="form-control" id="username" aria-describedby="usernameHelp" name="username">
                </div>
                <div class="mb-3">
                    <label for="password" class="form-label">Password</label>
                    <input type="password" class="form-control" id="password" name="password">
                </div>
                <button type="submit" class="btn btn-primary">Submit</button>
            </form>
        </div>
    `,
    data() {
        return {
            
        }
    },
    methods: {
        
    },
    watch: {
        
    },
    async mounted() {
        
    },
    computed:{
        
    }
});

