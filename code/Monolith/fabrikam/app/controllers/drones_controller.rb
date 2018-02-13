class DronesController < ApplicationController
  before_action :set_drone, only: [:show, :edit, :update, :destroy]

  # GET /drones
  # GET /drones.json
  def index
    @drones = Drone.all
  end

  # GET /drones/1
  # GET /drones/1.json
  def show
  end

  # GET /drones/new
  def new
    @drone = Drone.new
  end

  # GET /drones/1/edit
  def edit
  end

  # POST /drones
  # POST /drones.json
  def create
    @drone = Drone.new(drone_params)

    respond_to do |format|
      if @drone.save
        format.html { redirect_to @drone, notice: 'Drone was successfully created.' }
        format.json { render :show, status: :created, location: @drone }
      else
        format.html { render :new }
        format.json { render json: @drone.errors, status: :unprocessable_entity }
      end
    end
  end

  # PATCH/PUT /drones/1
  # PATCH/PUT /drones/1.json
  def update
    respond_to do |format|
      if @drone.update(drone_params)
        format.html { redirect_to @drone, notice: 'Drone was successfully updated.' }
        format.json { render :show, status: :ok, location: @drone }
      else
        format.html { render :edit }
        format.json { render json: @drone.errors, status: :unprocessable_entity }
      end
    end
  end

  # DELETE /drones/1
  # DELETE /drones/1.json
  def destroy
    @drone.destroy
    respond_to do |format|
      format.html { redirect_to drones_url, notice: 'Drone was successfully destroyed.' }
      format.json { head :no_content }
    end
  end

  private
    # Use callbacks to share common setup or constraints between actions.
    def set_drone
      @drone = Drone.find(params[:id])
    end

    # Never trust parameters from the scary internet, only allow the white list through.
    def drone_params
      params.require(:drone).permit(:state, :has_load)
    end
end
